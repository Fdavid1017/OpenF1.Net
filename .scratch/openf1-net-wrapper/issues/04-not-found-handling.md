Type: grilling
Status: resolved

## Question

The live API returns `404 {"detail":"No results found."}` when a query matches nothing. Should OpenF1.Net surface this as an exception, or as an empty result array?

## Answer

Empty array. A query matching zero rows is normal REST behavior, not an error condition — consumers shouldn't need a try/catch for "no laps yet this session." Every endpoint method returns `T[]`, and a `404` with this exact detail body maps to `[]`. Any other non-2xx status still follows [03-error-handling-model](03-error-handling-model.md).

## Comments
