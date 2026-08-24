Type: grilling
Status: resolved

## Question

Beyond DRS (see [07-drs-status-enum](07-drs-status-enum.md)), several response fields are documented with "..." (incomplete value lists): `race_control.category`, `race_control.flag`, `race_control.scope`, `stints.compound`, `sessions.session_type`, `sessions.session_name`, `laps.segments_sector_*`. Should these become C# enums, or stay `string` since the docs don't give exhaustive lists?

## Answer

Rule: a field becomes a C# enum **only if its full value set is actually confirmed** (by live API research during charting, or by the user directly); otherwise it stays `string`. Live research + user-supplied lists during charting confirmed the following are complete — every one below gets an enum, each with a custom `JsonConverter` that maps unrecognized/future raw values to a documented fallback member instead of throwing (same philosophy as DRS):

| Enum | Raw values confirmed | Fallback member |
|---|---|---|
| `Flag` | GREEN, DOUBLE YELLOW, YELLOW, RED, CLEAR, BLUE, CHEQUERED, BLACK AND WHITE | none needed — set is closed per user; keep field nullable (`Flag?`) since `race_control` rows outside category `Flag` have no flag value |
| `Scope` | Track, Sector, Driver | nullable (`Scope?`), same reasoning as `Flag` |
| `Category` (race_control) | SessionStatus, CarEvent, Drs, Flag, SafetyCar, Other | `Other` doubles as the catch-all — unrecognized future values also map to `Other`, no separate `Unknown` needed |
| `SessionName` | Day 1, Day 2, Day 3, Practice 1, Practice 2, Practice 3, Qualifying, Race, Sprint Qualifying, Sprint | n/a, closed set per user |
| `SessionType` | Practice, Qualifying, Race | n/a, closed set per user. Note: `Sprint`/`Sprint Qualifying` `session_name`s map to `session_type` `Race`/`Qualifying` respectively, not a distinct type — confirmed live, and the docs' own example for this is stale |
| `TyreCompound` | MEDIUM, TEST_UNKNOWN, SOFT, HARD, UNKNOWN, INTERMEDIATE, WET | n/a, closed set per user (includes API's own `UNKNOWN`/`TEST_UNKNOWN` literals) |
| `SegmentStatus` (`laps.segments_sector_*` array elements) | 0=Unavailable, 2048=Yellow, 2049=Green, 2051=Purple, 2064=Pitlane | `Unknown` — covers 2050/2052/2068 and anything else; the OpenF1 authors' own docs mark those three as `?`, they don't know either |
| `CircuitType` (`meetings.circuit_type`) | "Permanent", "Temporary - Street/Road" | n/a, closed set per docs |

Naming convention for multi-word raw values → PascalCase, spaces/punctuation stripped: `DOUBLE YELLOW` → `DoubleYellow`, `BLACK AND WHITE` → `BlackAndWhite`, `Sprint Qualifying` → `SprintQualifying`, `Temporary - Street/Road` → `TemporaryStreetRoad`.

Everything else observed (countries, circuit names/keys, driver names, team names/colours, message text, URLs, timestamps) stays `string`/appropriate primitive — open-ended sets, not enumerable.

## Comments
