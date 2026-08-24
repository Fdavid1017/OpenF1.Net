Type: grilling
Status: resolved

## Question

What testing strategy should OpenF1.Net adopt: unit tests against faked/mocked HTTP responses, integration tests hitting the live OpenF1 API, or both? Settle the mocking approach, test project structure/naming, what each layer covers, and how live-API tests (if any) are gated (e.g. CI, rate limits, flakiness).

## Answer

Two layers, one test project (`OpenF1.Net.Tests`, xUnit):

- **Unit tests** (`Unit/` folder): fast, deterministic, run on every build. HTTP responses faked via `RichardSzalay.MockHttp` injected as the `OpenF1` constructor's `HttpClient`. Covers all 18 endpoints with deep assertions (field values, enum conversion, filter builder query strings, error handling, rate limiter logic). Fake response bodies live in per-endpoint JSON fixture files under `Fixtures/` (e.g. `Fixtures/Drivers.json`), `CopyToOutputDirectory`, not inline strings or embedded resources.
- **Live tests** (`Live/` folder): opt-in, tagged `[Trait("Category","Live")]`, excluded from the default run (`dotnet test --filter "Category!=Live"`). Hits the real `https://api.openf1.org/v1` for all 18 endpoints (not a subset) with shallow assertions only (2xx + successful deserialization) — no CI wiring built (that's out of scope per [12-implement-library](12-implement-library.md)'s scope note), run manually or via a separate job later.

Rationale: implementation-phase live calls already caught 4 real bugs (rate-limiter boundary, nullable-enum converter, stints nullability, 404-detail parsing) that fake-only tests couldn't have, because the fakes would have been written from the same (occasionally wrong) assumptions as the code. Hitting the full endpoint set live, not a sample, is what made that catch-rate possible, so the live suite keeps that same full-coverage shape even though its assertions stay shallow. Plain xUnit `Assert.*`, no FluentAssertions/Shouldly — response DTOs are simple enough not to need it.

Checked for `CONTEXT.md`/`docs/adr/`: neither exists, and none of this map's 11 prior architecture decisions (HttpClient ownership, error handling, rate limiting, enum strategy, etc.) were promoted to a formal ADR — they live solely as closed wayfinder tickets, which is this repo's established record-keeping convention. This decision follows the same pattern; no ADR or glossary entry created.

## Comments

Implemented (2026-08-24): `OpenF1.Net.Tests` created exactly as specified — 48 unit tests (`Unit/`, all 18 endpoints plus filter builder, error handling, HTTP client ownership, `GetLatest*Async`, and rate limiter coverage) and 20 live tests (`Live/`, `[Trait("Category","Live")]`, one shared `OpenF1` instance per class via `IClassFixture` so the built-in rate limiter actually paces real calls instead of each test's own instance racing the others into a real 429).

Writing the tests immediately caught 3 more real bugs, same pattern as [12-implement-library](12-implement-library.md):

- `Lap.DurationSector1/2/3` and `Lap.SegmentsSector1/2/3` never deserialized: `JsonNamingPolicy.SnakeCaseLower` converts `DurationSector1` → `duration_sector1` (drops the underscore before a trailing digit), not the API's actual `duration_sector_1`. Fixed with explicit `[JsonPropertyName]` on the model, plus a new `ApiFieldNameAttribute` so `FilterBuilder`'s query-string field-name conversion (which has the identical bug) can be overridden per-property too — applied to `LapsFilterFields`.
- `PitStop.LaneDuration`/`PitDuration` were non-nullable `double` but live data returns `lane_duration: null` for at least one row; widened to `double?`.
- `SegmentStatus`'s converter called `reader.GetInt32()` unconditionally; live `segments_sector_1/2/3` arrays contain `null` entries (segment not yet reached), which threw. Now maps `null` → `SegmentStatus.Unavailable`, same meaning as raw value `0`.

All caught by the live suite failing against the real API on first run — the unit suite (built from the same, occasionally-wrong assumptions as the code) didn't and structurally couldn't have caught these, confirming the rationale this ticket was resolved on.
