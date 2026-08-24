Status: route clear — all 11 tickets resolved, nothing left blocking implementation. Remaining fog (see below) is deliberately deferred, non-blocking.

## Destination

Implementation-ready design spec for **OpenF1.Net** — a C# NuGet library wrapping every OpenF1 v1 REST endpoint (https://api.openf1.org/v1, docs https://openf1.org/docs) behind one instantiable `OpenF1` class, one partial class file per endpoint, strongly-typed request filters and response models. Reaching the end of this map means every architecture decision is locked and every response/enum field's typing is settled — ready to hand to a separate implementation effort. This map does not write the implementation itself (see [01-map-scope-decisions-only](issues/01-map-scope-decisions-only.md)).

## Notes

- Domain: C# / .NET 10 NuGet library development. Repo: `D:\Projects\OpenF1.Net`, existing `OpenF1.Net.csproj` targets `net10.0`, `GeneratePackageOnBuild=true`, nullable+implicit usings on.
- Skills to consult when resolving HITL tickets: `mattpocock-skills:grilling` + `mattpocock-skills:domain-modeling` for grilling-type tickets, `mattpocock-skills:prototype` for prototype-type tickets.
- Standing requirements carried from the original ask, not individual decisions, so recorded here instead of a ticket:
  - Main class name `OpenF1`, instantiable, one method per endpoint.
  - One partial class file per endpoint (method + anything endpoint-specific lives together) for readability.
  - Every response object gets its own class (e.g. `/drivers` → `Driver[]`); every classifiable value inside a response class gets its own class/enum where the full value set is known.
  - Doc comments and in-code comments: short, caveman-style, minimal — not a comment per line. Class properties get XML doc comments sourced from the OpenF1 docs' "Attributes" description text for that field.
  - Constructor takes optional `ILogger`; log at Error/Warning/Info where meaningful (see https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/overview).
- All 18 v1 endpoints and their raw field lists were captured via live docs fetch + live API queries during charting (2026-08-24); implementers should re-verify against https://openf1.org/docs before building, docs are occasionally stale vs. live behavior (confirmed: `session_type` for Sprint Qualifying sessions no longer matches the docs' own example).

## Decisions so far

- [Map scope is decisions-only](issues/01-map-scope-decisions-only.md): this map produces a design spec; actual coding happens in a separate, later effort.
- [HttpClient ownership](issues/02-httpclient-ownership.md): constructor takes optional `HttpClient?`; if null, `OpenF1` creates and owns (disposes) its own.
- [Error handling model](issues/03-error-handling-model.md): exception hierarchy (`OpenF1Exception` base), not a `Result<T>` wrapper.
- [404 "No results found" handling](issues/04-not-found-handling.md): treated as an empty array, not an exception.
- [Rate limiting](issues/05-rate-limiting-config.md): optional `OpenF1Config` with `UseRateLimit` (default `true`) drives a built-in client-side pacer; `OpenF1RateLimitException` on a real 429 regardless.
- [General enum strategy](issues/06-enum-strategy-general.md): fields keep type `string` unless the full value set is confirmed, in which case they become a fallback-safe C# enum. Nine fields qualified.
- [DRS status enum](issues/07-drs-status-enum.md): explicit numeric→enum folding (`DrsStatus.Off/Eligible/On`) with per-value doc comments, unmapped values default to `Off`.
- [Filter API shape](issues/08-filter-api-shape-fluent-builder.md): fluent builder (`.Where(x => x.LapDuration >= 120)`), not a plain filter-object-with-attributes.
- [Namespace/folder layout](issues/09-namespace-layout.md): `OpenF1.Net` / `.Models` / `.Models.Enums` / `.Filters` / `.Exceptions`.
- [Fluent filter builder design](issues/10-fluent-filter-builder-design.md): prototyped and validated — endpoint methods return a deferred, awaitable per-endpoint `XxxQuery` object (not `Task<T[]>` directly) with `.Where()/.And()/.WhereIn()`; OR works both via `WhereIn` and native `||` on the same equality-compared field (throws on non-equality or cross-field `||`); `latest` supported both as a sentinel struct in the builder and as a `GetLatestXAsync()` convenience method.
- [Polymorphic field modeling](issues/11-polymorphic-field-modeling.md): `GapToLeader` (seconds-or-"+N LAP" wrapper, with `IsLapped`/`ToString()`) used directly on `intervals`; `SessionResultGapToLeader`/`SessionResultDuration` (separate types, each with `Session`/`Q1`/`Q2`/`Q3` named properties) used on `session_result`.

## Not yet specified

- Per-endpoint response model field→C# type mapping for the ~16 straightforward endpoints (mechanical, guided by the field lists gathered during charting, but not individually specced/ticketed yet).
- NuGet package metadata polish (`PackageId`, `Version`, `Authors`, `License`, tags, icon) before first real publish.
- Testing strategy (unit/integration tests, live-API test fixtures, mocking approach) — not yet decided.
- Possible response caching layer — unrequested so far, may surface once usage patterns are clearer.

## Out of scope

- CSV export (`csv=true` passthrough) — contradicts the typed-object goal of this wrapper; never ticketed.
- OAuth2 bearer-token flow for real-time auth (`POST /token`, per docs) — live testing during charting showed real-time endpoints answering `200` without it; building a full OAuth flow is unrequested and the real requirement is unconfirmed. `OpenF1SubscriptionRequiredException` (see [03](issues/03-error-handling-model.md)) is a defensive placeholder only, not an OAuth implementation.
- CI/CD NuGet publish pipeline — only local `dotnet pack` (already enabled via `GeneratePackageOnBuild`); publish automation unrequested.
