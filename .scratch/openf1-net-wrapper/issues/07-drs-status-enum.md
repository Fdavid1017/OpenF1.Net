Type: grilling
Status: resolved

## Question

`car_data.drs` is a raw integer (0,1,2,3,8,9,10,12,14) with unclear meaning per-value. How should it fold into a `DrsStatus` enum?

## Answer

Given directly by the user, not grilled. Three-member enum, folded from the raw integers, each with a short XML doc comment:

- `DrsStatus.Off` ← raw 0, 1, 2, 3, 9 — doc comment: "Drs off"
- `DrsStatus.Eligible` ← raw 8 — doc comment: "Detected, eligible once in activation zone"
- `DrsStatus.On` ← raw 10, 12, 14 — doc comment: "Drs on"

Any raw value not in the above list (including genuinely unknown future values) falls back to `DrsStatus.Off`. Implemented via a custom `JsonConverter<DrsStatus>` on the `int` source field, same pattern generalized for the other enums in [06-enum-strategy-general](06-enum-strategy-general.md).

## Comments
