Type: prototype
Status: resolved

## Question

[08-filter-api-shape-fluent-builder](08-filter-api-shape-fluent-builder.md) settled on a fluent expression-based builder over a plain filter object, but not its exact public surface. Needs a rough, concrete stub to react to (per the `prototype` skill) covering:

- Expression grammar supported: property comparisons only (`x.Field == value`, `x.Field >= value`, etc.) — what operators map to which C# operators, and what happens if someone writes an unsupported expression shape (compile-time restriction via a narrow delegate type, or runtime `NotSupportedException`)?
- How array-typed response fields (e.g. `segments_sector_1`) are excluded from being filterable at the type level, per the docs' "filterable by any attribute except arrays" rule.
- How each endpoint's filter type gets generated/declared without repeating the builder plumbing per endpoint (18 endpoints) — likely a shared generic base (`FilterBuilder<TFilterable>`) plus one small filterable-fields marker type per endpoint.
- Date-valued field formatting into the query string (docs say the API parses several date formats leniently — pick one canonical outbound format, e.g. ISO 8601).
- `latest` sentinel support: `session_key`/`meeting_key` are typed `int` in responses but the API accepts the literal string `"latest"` in place of either. Decide how a consumer opts into that (e.g. `filter.Where(x => x.SessionKey == SessionKey.Latest)` via a special value type, or a separate `GetLatestSessionAsync()`-style convenience method per endpoint that needs it).
- How the built filter threads into each endpoint method's signature (e.g. `Task<Lap[]> GetLapsAsync(Action<LapsFilterBuilder>? filter = null, CancellationToken ct = default)`).

## Answer

Working prototype (not committed to any branch yet, sitting at [prototypes/10-filter-builder/Program.cs](../prototypes/10-filter-builder/Program.cs), runs via `dotnet run` in that folder) validated all six open items plus two the user raised mid-review:

- **Expression grammar**: `Where`/`And` take `Expression<Func<TFields, bool>>`, only direct binary comparisons (`==`, `>`, `>=`, `<`, `<=`) are parsed; anything else (e.g. `!=`) throws `NotSupportedException` at build time rather than being silently dropped — matches the API's actual supported operator set.
- **Array-field exclusion**: enforced at compile time, not runtime — each endpoint's `XxxFilterFields` marker class simply never declares a property for array-typed response fields (e.g. `segments_sector_1` isn't on `LapsFilterFields`), so filtering on one is a compile error, not a thrown exception.
- **Shared generic base**: `FilterBuilder<TFields>` holds all the parsing/formatting logic once; each endpoint gets one thin marker class (`LapsFilterFields`) plus a one-line derived builder (`class LapsFilterBuilder : FilterBuilder<LapsFilterFields>;`). No per-endpoint repetition of the parsing logic.
- **Date formatting**: canonical outbound format is `yyyy-MM-ddTHH:mm:ssZ` (UTC, ISO 8601).
- **`latest` sentinel** — user wants **both**: a `GetLatestSessionAsync()`-style convenience method on `OpenF1` for the common case, **and** the filter builder itself still understands a `SessionKeyRef`/`MeetingKeyRef` sentinel struct (implicit-int-convertible, `.Latest` static value) for when `latest` is needed inside a larger filter expression. Both stay, they're not mutually exclusive.
- **OR support** — user corrected the initial no-OR assumption: the live API *does* support OR, via repeating the same query key (`driver_number=1&driver_number=40`). Two ways in, both hitting the same underlying logic: `WhereIn<TValue>(x => x.DriverNumber, 1, 40)` for a values-list, and native `x.DriverNumber == 1 || x.DriverNumber == 40` inside `Where`/`And` (parses `OrElse` trees, requires every leaf to be `==`, requires every leaf to reference the same field). Either a non-equality leaf (`||` mixed with `>=`) or a leaf on a *different* field throws `NotSupportedException` with a specific message — the API can't express arbitrary boolean OR (only same-key-repeated equality), so the builder refuses rather than silently building a query that doesn't mean what it looks like.
- **Method signature / threading** — user asked whether `GetLapsAsync().Where(x => x.DriverNumber == 55)` (filtering *after* the call, before awaiting) is achievable, preferring it over an `Action<TBuilder>` callback if feasible. Prototyped and confirmed feasible (Case 7): `GetLapsAsync()` returns a per-endpoint `LapsQuery` object (not `Task<Lap[]>` directly) that exposes `.Where()`/`.And()`/`.WhereIn()` and implements the custom-awaitable pattern (`GetAwaiter()`), so the actual HTTP call is deferred until the expression is awaited. **This wins over the `Action<TBuilder>` fallback** — it's what the user asked for and it works. `CancellationToken` stays a normal parameter on the `GetLapsAsync(CancellationToken ct = default)` method, captured by the returned query object.

Net shape for real implementation: every endpoint method returns a small per-endpoint `XxxQuery` class (generic base for the awaitable+builder plumbing, thin per-endpoint subclass) instead of `Task<T[]>` directly.

## Comments
