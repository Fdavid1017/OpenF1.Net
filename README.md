[![Publish NuGet Package](https://github.com/Fdavid1017/OpenF1.Net/actions/workflows/nuget-push.yml/badge.svg?branch=master)](https://github.com/Fdavid1017/OpenF1.Net/actions/workflows/nuget-push.yml)

# OpenF1.Net

.NET wrapper for the [OpenF1 API](https://openf1.org/) ([docs](https://openf1.org/docs)), which provides real-time and historical Formula 1 data — sessions, drivers, lap times, car telemetry, positions, pit stops, weather, race control messages, and more.

Targets **.NET 10**.

## Why this package?

- **Strongly typed** — every endpoint returns its own model class (`Driver`, `Lap`, `Session`, ...), with enums (`Flag`, `TyreCompound`, `SessionType`, ...) instead of raw string values.
- **Expression-based filtering** — query parameters are written as LINQ-like lambda expressions (`x => x.DriverNumber == 1`), not as hand-concatenated query strings.
- **Deferred queries you can `await` directly** — `Get*Async` calls don't fire an HTTP request immediately; `.Where()`/`.And()`/`.WhereIn()` are chainable, and the request only runs when you `await` it.
- **Built-in rate limiting** — by default outgoing requests are throttled to stay under the API's 3 requests/second limit.
- **Explicit error handling** — dedicated exception types for rate limiting (429), authorization errors (401/403), and general API errors.

## Installation

The project produces a NuGet package on build (`GeneratePackageOnBuild`). As a package reference:

```bash
dotnet add package OpenF1.Net
```

or as a direct project reference if you build from source:

```bash
dotnet add reference path/to/OpenF1.Net/OpenF1.Net.csproj
```

## Usage

### Creating a client

```csharp
using OpenF1.Net;

await using var client = new OpenF1Client();
```

The `OpenF1Client` constructor optionally accepts an `HttpClient`, an `OpenF1Config` and an `ILogger`. If you don't supply an `HttpClient`, the wrapper creates and manages one (disposing it on `DisposeAsync()`).

```csharp
using Microsoft.Extensions.Logging;
using OpenF1.Net;

var httpClient = new HttpClient();
var config = new OpenF1Config { UseRateLimit = true }; // default
ILogger logger = loggerFactory.CreateLogger<OpenF1Client>();

await using var client = new OpenF1Client(httpClient, config, logger);
```

> With `UseRateLimit = false` it is the caller's responsibility to pace the requests — a real 429 response still throws `OpenF1RateLimitExceededException`.

### A simple query

Every `Get*Async` method returns a deferred `*Query` object that you can `await` directly — the HTTP request only starts at that point:

```csharp
Driver[] drivers = await client.GetDriversAsync();
```

### Filtering

Filter fields are provided through a dedicated `TFields` class using lambda expressions. Property names are converted to snake_case automatically (e.g. `DriverNumber` → `driver_number`).

```csharp
// A single condition
var verstappenLaps = await client.GetLapsAsync()
    .Where(x => x.DriverNumber == 1);

// Multiple conditions chained (AND)
var fastLaps = await client.GetLapsAsync()
    .Where(x => x.SessionKey == SessionKeyRef.Latest)
    .And(x => x.LapDuration < 90.0);

// Supported operators: ==, >, >=, <, <=
var longStints = await client.GetStintsAsync()
    .Where(x => x.LapEnd >= 20);
```

### "latest" references

The `session_key` and `meeting_key` fields accept the API's `latest` sentinel value through the `SessionKeyRef` / `MeetingKeyRef` implicit conversion:

```csharp
var currentSessionDrivers = await client.GetDriversAsync()
    .Where(x => x.SessionKey == SessionKeyRef.Latest);

// or via the dedicated convenience methods:
Session? latestSession = await client.GetLatestSessionAsync();
Meeting? latestMeeting = await client.GetLatestMeetingAsync();
```

### `WhereIn` — multiple values for one field (OR)

The OpenF1 API expresses OR by repeating the same query key (`driver_number=1&driver_number=44`). That's what `WhereIn` is for (the `||` operator also works, but only for equality comparisons within a single field):

```csharp
var selectedDrivers = await client.GetDriversAsync()
    .WhereIn(x => x.DriverNumber, 1, 11, 44);

// equivalent to:
var sameResult = await client.GetDriversAsync()
    .Where(x => x.DriverNumber == 1 || x.DriverNumber == 11 || x.DriverNumber == 44);
```

### Circuit details (`IncludeCircuitInfo`)

`.IncludeCircuitInfo()`, chainable onto a `GetMeetingsAsync()` query, optionally fetches detailed circuit data for each meeting (corners, marshal posts, pit lane loss, track outline) from `Meeting.CircuitInfoUrl` — this is not served by the OpenF1 API but by MultiViewer. It costs one extra HTTP request per meeting, so it is off by default: without `.IncludeCircuitInfo()` the `Meeting.CircuitInfo` property is `null`.

```csharp
var meetings = await client.GetMeetingsAsync().IncludeCircuitInfo();

foreach (var meeting in meetings)
{
    var info = meeting.CircuitInfo!;
    Console.WriteLine($"{info.CircuitName}: {info.Corners.Length} corners, pit loss {info.PitLoss.Normal}s");
}
```

### Driver images (`ResolveImages`)

`.ResolveImages()`, chainable onto a `GetDriversAsync()` query, optionally resolves the highest available resolution official F1 headshot URL per driver, and fills the `Driver.FullBodyUrlLeft` / `Driver.FullBodyUrlRight` fields with the left and right full-body images for the driver's current team. This does not call the OpenF1 API but media.formula1.com (primary source) and assets.multiviewer.dev (fallback, when the official image is not found) — using `HEAD` requests only to check whether a given URL exists; the image itself is never downloaded. This can mean several extra HTTP requests per driver, so it is off by default: without `.ResolveImages()`, `Driver.HeadshotUrl` stays the value returned by the OpenF1 API, and `FullBodyUrlLeft`/`FullBodyUrlRight` are `null`.

```csharp
var drivers = await client.GetDriversAsync().ResolveImages();

foreach (var driver in drivers)
{
    Console.WriteLine($"{driver.FullName}: {driver.HeadshotUrl}");
    Console.WriteLine($"  left: {driver.FullBodyUrlLeft}, right: {driver.FullBodyUrlRight}");
}
```

### Team car images (`GetChampionshipTeamsAsync`)

`GetChampionshipTeamsAsync()` automatically (with no opt-in call) fills the `ChampionshipTeam.CarLeftUrl` / `ChampionshipTeam.CarRightUrl` fields for every team with the URL of the left and right render of the current car — from media.formula1.com, checking existence with a `HEAD` request, without downloading the image. If no suitable image is found for a team, the field stays `null`.

```csharp
var teams = await client.GetChampionshipTeamsAsync();

foreach (var team in teams)
    Console.WriteLine($"{team.TeamName}: {team.CarLeftUrl}");
```

### Enums

Some fields (`Flag`, `TyreCompound`, `SessionType`, `Category`, `Scope`, ...) arrive as strongly typed enums, and the raw API string in the response (e.g. `"BLACK AND WHITE"`) is mapped automatically to the matching member (`Flag.BlackAndWhite`) — when filtering, the same happens in reverse.

```csharp
var blackAndWhiteFlags = await client.GetRaceControlAsync()
    .Where(x => x.Flag == Flag.BlackAndWhite);

var softTyreStints = await client.GetStintsAsync()
    .Where(x => x.Compound == TyreCompound.Soft);
```

### Error handling

```csharp
using OpenF1.Net.Exceptions;

try
{
    var laps = await client.GetLapsAsync().Where(x => x.SessionKey == SessionKeyRef.Latest);
}
catch (OpenF1RateLimitExceededException)
{
    // HTTP 429 — the API's 3 requests/second limit
}
catch (OpenF1SubscriptionRequiredException)
{
    // HTTP 401/403 — endpoint that requires a subscription
}
catch (OpenF1ApiException ex)
{
    // any other non-2xx response
    Console.WriteLine($"{ex.StatusCode}: {ex.Detail}");
}
```

## Available endpoints

| Method | Description |
|---|---|
| `GetCarDataAsync()` | Car telemetry (RPM, speed, gear, throttle/brake, DRS), sampled at ~3.7 Hz. |
| `GetChampionshipDriversAsync()` | Championship standings for the drivers. Available at race events only. |
| `GetChampionshipTeamsAsync()` | Championship standings for the teams. Available at race events only. |
| `GetDriversAsync()` | Driver data for a given session. |
| `GetIntervalsAsync()` | Real-time gap between the drivers and the leader. Available at race events only. |
| `GetLapsAsync()` | Detailed lap time data. |
| `GetLocationAsync()` | Approximate track position of the cars, sampled at ~3.7 Hz. |
| `GetMeetingsAsync()` | Data for a given weekend (Grand Prix or test). |
| `GetOvertakesAsync()` | Overtakes. Available at race events only, and may be incomplete. |
| `GetPitAsync()` | Pit stops. |
| `GetPositionAsync()` | How positions evolve during the session. |
| `GetRaceControlAsync()` | Race control messages (session status, incidents, flags, safety car, ...). |
| `GetSessionResultAsync()` | Session results. Becomes available a few minutes after the official results are published. |
| `GetSessionsAsync()` | Session data (practice, qualifying, sprint, race, ...). |
| `GetStartingGridAsync()` | The starting grid for the upcoming race. |
| `GetStintsAsync()` | Data for the individual stints (continuous driving segments). |
| `GetTeamRadioAsync()` | Team radio exchanges (selected recordings only). |
| `GetWeatherAsync()` | Weather data over the track, updated every minute. |
| `GetLatestSessionAsync()` | Convenience method for fetching the current/most recent session (`session_key=latest`). |
| `GetLatestMeetingAsync()` | Convenience method for fetching the current/most recent meeting (`meeting_key=latest`). |

## Full example

```csharp
using OpenF1.Net;
using OpenF1.Net.Exceptions;
using OpenF1.Net.Filters;

await using var client = new OpenF1Client();

try
{
    var session = await client.GetLatestSessionAsync();
    if (session is null)
    {
        Console.WriteLine("No active session.");
        return;
    }

    Console.WriteLine($"{session.CircuitShortName} — {session.SessionName} ({session.Year})");

    var drivers = await client.GetDriversAsync()
        .Where(x => x.SessionKey == SessionKeyRef.Latest);

    foreach (var driver in drivers)
        Console.WriteLine($"#{driver.DriverNumber,-3} {driver.FullName,-25} {driver.TeamName}");

    var fastestLaps = await client.GetLapsAsync()
        .Where(x => x.SessionKey == SessionKeyRef.Latest)
        .And(x => x.LapDuration < 95.0);

    foreach (var lap in fastestLaps)
        Console.WriteLine($"#{lap.DriverNumber} lap {lap.LapNumber}: {lap.LapDuration}s");
}
catch (OpenF1RateLimitExceededException)
{
    Console.WriteLine("Too many requests — try again later.");
}
```

## Tests

```bash
dotnet test
```

The `OpenF1.Net.Tests` project contains both unit tests (with recorded JSON fixtures, `OpenF1.Net.Tests/Fixtures`) and live endpoint tests (`OpenF1.Net.Tests/Live`).

## Links

- OpenF1 API: https://openf1.org/
- OpenF1 API documentation: https://openf1.org/docs
- OpenF1 source: https://github.com/br-g/openf1

## Managing releases (git-cliff)

Versioning is based on [Conventional Commits](https://www.conventionalcommits.org/); the
changelog and the next version number are generated by
[git-cliff](https://git-cliff.org/docs/). The configuration is `cliff.toml` in the repo root.

### Commit convention

`feat:` bumps the minor version, `fix:`/`perf:`/`refactor:` bump the patch version, and a
`BREAKING CHANGE:` footer (or `feat!:`) bumps the major version. `chore:` and `style:`
commits are left out of the changelog.

### Local usage

```bash
npx git-cliff --bumped-version          # what the next version would be
npx git-cliff --unreleased              # what would go into the next release
npx git-cliff --tag v1.2.0 -o CHANGELOG.md
```

(Alternative installs: `winget install git-cliff`, `brew install git-cliff` or
`cargo install git-cliff`.)

### Publishing a release

On the `master` branch the **Release** workflow (`.github/workflows/release.yml`) is
triggered manually (Actions → Release → Run workflow):

1. it computes the next version (or uses the supplied `version` input),
2. updates `CHANGELOG.md` and commits it as `chore(release): prepare for vX.Y.Z`,
3. creates and pushes the `vX.Y.Z` tag,
4. runs `dotnet pack -p:Version=X.Y.Z` and pushes to GitHub Packages,
5. creates a GitHub Release with the generated release notes.

The `dry_run` input lets you inspect the computed version and the changelog without
publishing anything.

### Package release notes

The workflows write the output of `git cliff --unreleased --strip all` into
`RELEASE_NOTES.md`, and the csproj's `SetPackageReleaseNotes` target loads that into the
nuspec `<releaseNotes>` field. This way the "Release Notes" section on the NuGet package
page automatically shows the commits belonging to that version. If `RELEASE_NOTES.md` is
not in the repo root (a plain local build), the field stays empty.

It can be reproduced locally like this (`&&` does not work in Windows PowerShell 5.1, hence
two separate commands):

```bash
npx git-cliff --unreleased --strip all -o RELEASE_NOTES.md
```

```bash
dotnet pack OpenF1.Net.csproj -c Release -o ./nupkg
```

### Develop prereleases

Commits pushed to the `develop` branch automatically produce a prerelease package
(`.github/workflows/nuget-push.yml`). The version is the *next stable* version computed by
git-cliff, with a `-dev.<timestamp>` suffix — e.g. if develop has a `feat:` commit since the
`v0.1.0` tag, the package is published as `0.2.0-dev.20260825171044`. Per SemVer this is a
prerelease, so NuGet only offers it when prerelease packages are enabled, and it always
sorts lower than the eventual stable `0.2.0`.
