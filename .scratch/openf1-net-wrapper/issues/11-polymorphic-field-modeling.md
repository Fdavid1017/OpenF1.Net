Type: grilling
Status: resolved

## Question

A handful of response fields are polymorphic in the raw JSON — same field, different JSON type depending on context:

- `intervals.gap_to_leader` / `intervals.interval`: `float` (seconds) or `string` (`"+1 LAP"`)
- `session_result.duration`: `float` (a single lap/race time) or `array` (multiple values, e.g. per-Q1/Q2/Q3 in qualifying)
- `session_result.gap_to_leader`: `float`, `string` (`"+N LAP(S)"`), or `array`

How should these be modeled in C# so they stay strongly typed rather than falling back to `object`/`JsonElement`?

Candidate directions to weigh: (a) a small discriminated-union-style wrapper type per shape (e.g. `GapToLeader` with `double? Seconds`, `string? LapsBehind`, an `IsGap`/`IsLapped` discriminator, and a custom `JsonConverter`), (b) dual nullable properties (`double? DurationSeconds` + `string? DurationRaw`) with no wrapper type, (c) something else. Needs to cover both the two-shape fields (`intervals.*`) and the three-shape fields (`session_result.duration`/`gap_to_leader`, which also has an array case for multi-segment qualifying results).

## Answer

Two distinct types, not one shared shape, plus convenience members on the leaf type.

**`GapToLeader`** — the pure two-shape wrapper (`float` seconds *or* `string` "+N LAP(S)"), used directly as `intervals.gap_to_leader`'s and `intervals.interval`'s property type. Never carries qualifying segments — that structure lives one level up (see below), not folded into this type.

```csharp
readonly struct GapToLeader
{
    public double? Seconds { get; }
    public string? LapsBehind { get; } // raw "+N LAP(S)" text
    public bool IsLapped => LapsBehind is not null;
    public override string ToString() => LapsBehind ?? $"{Seconds:0.000}s";
}
```

Custom `JsonConverter<GapToLeader>`: a JSON number → `Seconds`; a JSON string → `LapsBehind`; `null` → both null.

**`SessionResultGapToLeader`** — the three-shape `session_result.gap_to_leader` field (scalar `GapToLeader` for race/practice, or a `[Q1, Q2, Q3]` array for qualifying). Named properties, not a generic list — there are never more than 3 qualifying segments, so `Q1`/`Q2`/`Q3` read better than indexing:

```csharp
class SessionResultGapToLeader
{
    public GapToLeader? Session { get; init; }
    public double? Q1 { get; init; }
    public double? Q2 { get; init; }
    public double? Q3 { get; init; }
}
```

`Q1`/`Q2`/`Q3` are plain `double?` (seconds only) — the user confirmed the qualifying-segment values never carry the "+N LAP" text form, only the top-level `Session` value can. Its `JsonConverter`: scalar token (number or string) → `Session` via the same number-or-string parse as `GapToLeader`, `Q1`/`Q2`/`Q3` left `null`; array token → zip elements 0/1/2 into `Q1`/`Q2`/`Q3` (each element parsed as a plain number, `Session` left `null`).

**`SessionResultDuration`** — same shape as `SessionResultGapToLeader` but for `session_result.duration`, which docs only ever describe as `float` or `array` (no string/"+N LAP" case), so it's plain doubles throughout, no `GapToLeader` involved:

```csharp
class SessionResultDuration
{
    public double? Session { get; init; }
    public double? Q1 { get; init; }
    public double? Q2 { get; init; }
    public double? Q3 { get; init; }
}
```

Not shared between `intervals` and `session_result` — user confirmed these should be separate types even though `GapToLeader` itself is reused as a building block (directly on `intervals`, and as `SessionResultGapToLeader.Session`).

## Comments
