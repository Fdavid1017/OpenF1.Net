Type: grilling
Status: resolved

## Question

What namespace/folder layout for the project?

## Answer

- `OpenF1.Net` (root) — the `OpenF1` main class, one partial class file per endpoint (e.g. `OpenF1.Drivers.cs`, `OpenF1.Laps.cs`, ...)
- `OpenF1.Net.Models` — one response class per endpoint (`Driver`, `Lap`, `Meeting`, ...)
- `OpenF1.Net.Models.Enums` — `DrsStatus`, `Flag`, `Scope`, `Category`, `SessionName`, `SessionType`, `TyreCompound`, `SegmentStatus`, `CircuitType`
- `OpenF1.Net.Filters` — the fluent filter builder (see [10-fluent-filter-builder-design](10-fluent-filter-builder-design.md))
- `OpenF1.Net.Exceptions` — `OpenF1Exception` hierarchy (see [03-error-handling-model](03-error-handling-model.md))

## Comments
