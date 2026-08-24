Type: grilling
Status: resolved

## Question

Every endpoint supports filtering any non-array response field via URL query params with operators (`=`, `<`, `>`, `<=`, `>=`), e.g. `?driver_number=55&lap_duration>=120`. What's the generic, low-repetition shape for exposing this in C#, shared across all 18 endpoints?

## Answer

Fluent builder, not a plain attribute-decorated filter-object. Consumer writes something like:

```csharp
openF1.GetLapsAsync(filter => filter.Where(x => x.DriverNumber == 55).And(x => x.LapDuration >= 120));
```

High-level shape only decided here — the exact builder surface (supported expression grammar, operator mapping, exclusion of array-typed fields from filterability, date-value formatting, `latest` sentinel support for `session_key`/`meeting_key`) is substantial enough to need its own round; see [10-fluent-filter-builder-design](10-fluent-filter-builder-design.md) (open).

## Comments
