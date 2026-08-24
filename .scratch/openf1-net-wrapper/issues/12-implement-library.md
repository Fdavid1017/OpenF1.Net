Type: task
Status: resolved

## Question

Build the actual OpenF1.Net library from the 11 resolved architecture tickets plus the full 18-endpoint field list (pulled from openF1-documentation.pdf during charting) — this map's destination.

## Answer

Implemented in full: `OpenF1` main class (ctor, IAsyncDisposable, HTTP execution core), `OpenF1Config`, the exception hierarchy, the fluent filter builder promoted from the [10-fluent-filter-builder-design](10-fluent-filter-builder-design.md) prototype (collapsed the prototype's per-endpoint Query subclasses into one generic `EndpointQuery<TFields, TModel>` base, since they added no logic beyond parameterization), all 9 enums with their JsonConverters, the 3 polymorphic wrapper types, and all 18 endpoints (model + FilterFields + Query + partial method). `dotnet build`/`dotnet pack` clean, 0 warnings.

Verified against the live API (not just synthetic prototype cases) — found and fixed 4 real bugs the docs/tickets couldn't have caught:

- **Rate limiter boundary bug**: fixed-interval spacing (1000/3 ms apart) still let 4 requests land inside one rolling 1-second window at the boundary (0, 333, 666, 1000ms) and tripped the API's real 429. Replaced with a proper sliding-window limiter (queue of the last 3 request timestamps, wait for the oldest to age out).
- **`Nullable<TEnum>` + `[JsonConverter]` doesn't work for null-fallback converters**: System.Text.Json's `NullableConverterFactory` always resolves a converter for the plain non-nullable enum first, so a `[JsonConverter]` attribute targeting `Flag?`/`Scope?` directly throws `InvalidOperationException` the moment anything touches `Flag`. Fixed by registering `NullableFlagJsonConverter`/`NullableScopeJsonConverter` explicitly in `OpenF1`'s `JsonSerializerOptions.Converters` instead of via type attribute — every other converter here targets its plain non-nullable type so it's unaffected.
- **`stints.lap_start`/`lap_end` are nullable in practice** despite the docs implying always-set (an in-progress stint has no `lap_end` yet; live data also showed `lap_start` null in at least one case). Widened both to `int?`.
- **404-detail parsing used the wrong `JsonSerializerOptions`**: `TryParseDetail` called `JsonSerializer.Deserialize<ApiErrorResponse>(body)` with the framework defaults (case-sensitive, no naming policy), so `"detail"` never bound to the `Detail` property and the 404→empty-array path never triggered — every non-2xx response fell through to the generic `OpenF1ApiException`. Fixed by passing the shared case-insensitive `JsonOptions`.

Also confirmed live that `circuit_type` has a third real value beyond the two the docs list — `"Temporary - Street"` (e.g. current Singapore), distinct from `"Temporary - Street/Road"`. Added `CircuitType.TemporaryStreet` as a confirmed member rather than relying on the strict-throw fallback, since this is now directly observed, not a hypothetical future value.

## Comments
