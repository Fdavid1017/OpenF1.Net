[![Publish NuGet Package](https://github.com/Fdavid1017/OpenF1.Net/actions/workflows/nuget-push.yml/badge.svg?branch=master)](https://github.com/Fdavid1017/OpenF1.Net/actions/workflows/nuget-push.yml)

# OpenF1.Net

.NET wrapper for the [OpenF1 API](https://openf1.org/) ([docs](https://openf1.org/docs)), which provides real-time and historical Formula 1 data — sessions, drivers, lap times, car telemetry, positions, pit stops, weather, race control messages, and more.

Targets **.NET 10**.

## Miért ezt a csomagot?

- **Erősen típusos** — minden végpont saját modellosztályt ad vissza (`Driver`, `Lap`, `Session`, ...), enumokkal (`Flag`, `TyreCompound`, `SessionType`, ...) a nyers string értékek helyett.
- **Kifejezésalapú szűrés** — a query paramétereket LINQ-szerű lambda kifejezésekkel írod (`x => x.DriverNumber == 1`), nem kézzel összefűzött query stringekkel.
- **Halasztott, közvetlenül `await`-elhető lekérdezések** — a `Get*Async` hívások nem indítanak azonnal HTTP kérést; a `.Where()`/`.And()`/`.WhereIn()` láncolható, és a kérés csak `await`-eléskor fut le.
- **Beépített rate limiting** — alapértelmezésben az API 3 kérés/másodperc korlátja alá szabályozza a kimenő kéréseket.
- **Explicit hibakezelés** — dedikált kivételtípusok a rate limitre (429), jogosultsági hibákra (401/403) és az általános API hibákra.

## Telepítés

A projekt NuGet csomagot generál build közben (`GeneratePackageOnBuild`). Projekt-referenciaként:

```bash
dotnet add package OpenF1.Net
```

vagy közvetlen projekt-referenciaként, ha a forrásból építed:

```bash
dotnet add reference path/to/OpenF1.Net/OpenF1.Net.csproj
```

## Használat

### Kliens létrehozása

```csharp
using OpenF1.Net;

await using var client = new OpenF1Client();
```

Az `OpenF1Client` konstruktora opcionálisan elfogad egy `HttpClient`-et, egy `OpenF1Config`-ot és egy `ILogger`-t. Ha nem adsz meg `HttpClient`-et, a wrapper létrehoz és kezel egyet (dispose-olja `DisposeAsync()`-kor).

```csharp
using Microsoft.Extensions.Logging;
using OpenF1.Net;

var httpClient = new HttpClient();
var config = new OpenF1Config { UseRateLimit = true }; // alapértelmezett
ILogger logger = loggerFactory.CreateLogger<OpenF1Client>();

await using var client = new OpenF1Client(httpClient, config, logger);
```

> `UseRateLimit = false` esetén a hívó felelőssége a kérések ütemezése — egy valódi 429-es válasz így is `OpenF1RateLimitExceededException`-t dob.

### Egyszerű lekérdezés

Minden `Get*Async` metódus egy halasztott `*Query` objektumot ad vissza, amit közvetlenül `await`-elhetsz — a HTTP kérés csak ekkor indul el:

```csharp
Driver[] drivers = await client.GetDriversAsync();
```

### Szűrés

A szűrő mezők egy dedikált `TFields` osztályon keresztül, lambda kifejezésekkel adhatók meg. A property nevek automatikusan snake_case-re alakulnak (pl. `DriverNumber` → `driver_number`).

```csharp
// Egyetlen feltétel
var verstappenLaps = await client.GetLapsAsync()
    .Where(x => x.DriverNumber == 1);

// Több feltétel láncolva (AND)
var fastLaps = await client.GetLapsAsync()
    .Where(x => x.SessionKey == SessionKeyRef.Latest)
    .And(x => x.LapDuration < 90.0);

// Támogatott operátorok: ==, >, >=, <, <=
var longStints = await client.GetStintsAsync()
    .Where(x => x.LapEnd >= 20);
```

### "latest" hivatkozások

A `session_key` és `meeting_key` mezők elfogadják az API `latest` szentinel értékét a `SessionKeyRef` / `MeetingKeyRef` implicit konverzión keresztül:

```csharp
var currentSessionDrivers = await client.GetDriversAsync()
    .Where(x => x.SessionKey == SessionKeyRef.Latest);

// vagy közvetlen kényelmi metódusokkal:
Session? latestSession = await client.GetLatestSessionAsync();
Meeting? latestMeeting = await client.GetLatestMeetingAsync();
```

### `WhereIn` — több érték egy mezőre (OR)

Az OpenF1 API az OR-t ugyanazon query kulcs ismétlésével fejezi ki (`driver_number=1&driver_number=44`). Erre a `WhereIn` szolgál (a `||` operátor is működik, de csak egyetlen mezőn belüli egyenlőség-összehasonlításokra):

```csharp
var selectedDrivers = await client.GetDriversAsync()
    .WhereIn(x => x.DriverNumber, 1, 11, 44);

// ezzel ekvivalens:
var sameResult = await client.GetDriversAsync()
    .Where(x => x.DriverNumber == 1 || x.DriverNumber == 11 || x.DriverNumber == 44);
```

### Pálya részletek (`IncludeCircuitInfo`)

A `GetMeetingsAsync()` lekérdezésre láncolható `.IncludeCircuitInfo()` opcionálisan lekéri az egyes meetingekhez tartozó részletes pályaadatokat (kanyarok, marsall-posztok, boxutca-veszteség, pályakontúr) a `Meeting.CircuitInfoUrl`-ről — ezt nem az OpenF1 API szolgáltatja, hanem a MultiViewer. Meetingenként egy plusz HTTP kérést jelent, ezért alapból ki van kapcsolva: `.IncludeCircuitInfo()` nélkül a `Meeting.CircuitInfo` property `null`.

```csharp
var meetings = await client.GetMeetingsAsync().IncludeCircuitInfo();

foreach (var meeting in meetings)
{
    var info = meeting.CircuitInfo!;
    Console.WriteLine($"{info.CircuitName}: {info.Corners.Length} kanyar, boxutca-veszteség {info.PitLoss.Normal}s");
}
```

### Driver-képek (`ResolveImages`)

A `GetDriversAsync()` lekérdezésre láncolható `.ResolveImages()` opcionálisan feloldja driverenként a legnagyobb elérhető felbontású hivatalos F1 headshot URL-t, és kitölti a `Driver.FullBodyUrlLeft` / `Driver.FullBodyUrlRight` mezőket a driver aktuális csapatához tartozó egész alakos, bal illetve jobb oldali képekkel. Ehhez nem az OpenF1 API-t hívjuk, hanem a media.formula1.com-ot (elsődleges forrás) és az assets.multiviewer.dev-et (tartalék, ha a hivatalos kép nem található) — kizárólag `HEAD` kéréssel ellenőrizve, hogy egy adott URL létezik-e, a kép ténylegesen sosem töltődik le. Ez driverenként több extra HTTP kérést jelenthet, ezért alapból ki van kapcsolva: `.ResolveImages()` nélkül a `Driver.HeadshotUrl` az OpenF1 API által visszaadott érték marad, a `FullBodyUrlLeft`/`FullBodyUrlRight` pedig `null`.

```csharp
var drivers = await client.GetDriversAsync().ResolveImages();

foreach (var driver in drivers)
{
    Console.WriteLine($"{driver.FullName}: {driver.HeadshotUrl}");
    Console.WriteLine($"  bal: {driver.FullBodyUrlLeft}, jobb: {driver.FullBodyUrlRight}");
}
```

### Csapat autó-képek (`GetChampionshipTeamsAsync`)

A `GetChampionshipTeamsAsync()` minden csapathoz automatikusan (opt-in hívás nélkül) feltölti a `ChampionshipTeam.CarLeftUrl` / `ChampionshipTeam.CarRightUrl` mezőket az aktuális autó bal, illetve jobb oldali renderjének URL-jével — a media.formula1.com-ról, `HEAD` kéréssel ellenőrizve a létezést, a kép letöltése nélkül. Ha egy csapathoz nem található megfelelő kép, a mező `null` marad.

```csharp
var teams = await client.GetChampionshipTeamsAsync();

foreach (var team in teams)
    Console.WriteLine($"{team.TeamName}: {team.CarLeftUrl}");
```

### Enumok

Néhány mező (`Flag`, `TyreCompound`, `SessionType`, `Category`, `Scope`, ...) erősen típusos enumként érkezik, és a válaszban lévő nyers API string (pl. `"BLACK AND WHITE"`) automatikusan a megfelelő tagra (`Flag.BlackAndWhite`) van leképezve — szűréskor ugyanez fordítva történik.

```csharp
var blackAndWhiteFlags = await client.GetRaceControlAsync()
    .Where(x => x.Flag == Flag.BlackAndWhite);

var softTyreStints = await client.GetStintsAsync()
    .Where(x => x.Compound == TyreCompound.Soft);
```

### Hibakezelés

```csharp
using OpenF1.Net.Exceptions;

try
{
    var laps = await client.GetLapsAsync().Where(x => x.SessionKey == SessionKeyRef.Latest);
}
catch (OpenF1RateLimitExceededException)
{
    // HTTP 429 — az API 3 kérés/másodperc korlátja
}
catch (OpenF1SubscriptionRequiredException)
{
    // HTTP 401/403 — előfizetést igénylő végpont
}
catch (OpenF1ApiException ex)
{
    // egyéb, nem 2xx válasz
    Console.WriteLine($"{ex.StatusCode}: {ex.Detail}");
}
```

## Elérhető végpontok

| Metódus | Leírás |
|---|---|
| `GetCarDataAsync()` | Autótelemetria (fordulatszám, sebesség, sebességfokozat, gázpedál/fék, DRS), ~3.7 Hz mintavétellel. |
| `GetChampionshipDriversAsync()` | Bajnoki állás a versenyzők számára. Csak versenyeken elérhető. |
| `GetChampionshipTeamsAsync()` | Bajnoki állás a csapatok számára. Csak versenyeken elérhető. |
| `GetDriversAsync()` | Egy adott session-höz tartozó versenyzők adatai. |
| `GetIntervalsAsync()` | Valós idejű időrés a versenyzők és az élen haladó között. Csak versenyeken elérhető. |
| `GetLapsAsync()` | Részletes köridő adatok. |
| `GetLocationAsync()` | Az autók hozzávetőleges pályahelyzete, ~3.7 Hz mintavétellel. |
| `GetMeetingsAsync()` | Egy adott hétvégéhez (Grand Prix vagy teszt) tartozó adatok. |
| `GetOvertakesAsync()` | Előzések. Csak versenyeken elérhető, és lehet hiányos. |
| `GetPitAsync()` | Boxkiállások. |
| `GetPositionAsync()` | Helyezések alakulása a session során. |
| `GetRaceControlAsync()` | Versenyirányítási üzenetek (session állapot, incidensek, zászlók, biztonsági autó, ...). |
| `GetSessionResultAsync()` | Session eredmények. Néhány perccel a hivatalos eredmény kihirdetése után válik elérhetővé. |
| `GetSessionsAsync()` | Session-ök adatai (edzés, időmérő, sprint, verseny, ...). |
| `GetStartingGridAsync()` | A rajtrács a következő versenyhez. |
| `GetStintsAsync()` | Az egyes stint-ek (folyamatos vezetési szakaszok) adatai. |
| `GetTeamRadioAsync()` | Csapatrádió-beszélgetések (csak válogatott felvételek). |
| `GetWeatherAsync()` | Időjárási adatok a pálya felett, percenkénti frissítéssel. |
| `GetLatestSessionAsync()` | Kényelmi metódus a jelenlegi/legutóbbi session lekérésére (`session_key=latest`). |
| `GetLatestMeetingAsync()` | Kényelmi metódus a jelenlegi/legutóbbi meeting lekérésére (`meeting_key=latest`). |

## Teljes példa

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
        Console.WriteLine("Nincs aktív session.");
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
        Console.WriteLine($"#{lap.DriverNumber} kör {lap.LapNumber}: {lap.LapDuration}s");
}
catch (OpenF1RateLimitExceededException)
{
    Console.WriteLine("Túl sok kérés — próbáld később.");
}
```

## Tesztek

```bash
dotnet test
```

Az `OpenF1.Net.Tests` projekt unit teszteket (rögzített JSON fixture-ökkel, `OpenF1.Net.Tests/Fixtures`) és élő végpont teszteket (`OpenF1.Net.Tests/Live`) is tartalmaz.

## Linkek

- OpenF1 API: https://openf1.org/
- OpenF1 API dokumentáció: https://openf1.org/docs
- OpenF1 forrás: https://github.com/br-g/openf1

## Release-ek kezelése (git-cliff)

A verziózás [Conventional Commits](https://www.conventionalcommits.org/) alapú, a
changelog és a következő verziószám generálását a [git-cliff](https://git-cliff.org/docs/)
végzi. A konfiguráció a repo gyökerében lévő `cliff.toml`.

### Commit konvenció

A `feat:` minor, a `fix:`/`perf:`/`refactor:` patch verziót emel, a `BREAKING CHANGE:`
lábjegyzet (vagy `feat!:`) major-t. A `chore:` és `style:` commitok kimaradnak a changelogból.

### Helyi használat

```bash
npx git-cliff --bumped-version          # mi lenne a következő verzió
npx git-cliff --unreleased              # mi kerülne a következő release-be
npx git-cliff --tag v1.2.0 -o CHANGELOG.md
```

(Alternatív telepítés: `winget install git-cliff`, `brew install git-cliff` vagy
`cargo install git-cliff`.)

### Release kiadása

A `master` ágon a **Release** workflow (`.github/workflows/release.yml`) indítható
kézzel (Actions → Release → Run workflow):

1. kiszámolja a következő verziót (vagy a megadott `version` inputot használja),
2. frissíti a `CHANGELOG.md`-t és commitolja `chore(release): prepare for vX.Y.Z` néven,
3. létrehozza és pusholja a `vX.Y.Z` taget,
4. `dotnet pack -p:Version=X.Y.Z` és push a GitHub Packages-re,
5. GitHub Release-t hoz létre a generált release-jegyzettel.

A `dry_run` inputtal minden publikálás nélkül megnézhető a számolt verzió és a changelog.

### Csomag release note

A workflow-k a `git cliff --unreleased --strip all` kimenetét `RELEASE_NOTES.md`-be írják,
a csproj `SetPackageReleaseNotes` targetje pedig ezt tölti be a nuspec `<releaseNotes>`
mezőjébe. Így a NuGet csomag oldalán a "Release Notes" szekció automatikusan az adott
verzióhoz tartozó commitokat mutatja. Ha a `RELEASE_NOTES.md` nincs a repo gyökerében
(sima lokális build), a mező üresen marad.

Lokálisan így reprodukálható (a `&&` Windows PowerShell 5.1-ben nem működik,
ezért két külön parancs):

```bash
npx git-cliff --unreleased --strip all -o RELEASE_NOTES.md
```

```bash
dotnet pack OpenF1.Net.csproj -c Release -o ./nupkg
```

A `develop` ágra pusholt commitokból továbbra is automatikus prerelease csomag készül
(`.github/workflows/nuget-push.yml`).
