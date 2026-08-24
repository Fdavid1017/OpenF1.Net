Type: grilling
Status: resolved

## Question

The live API caps requests at 3/second and returns `429` past that. Should OpenF1.Net throttle client-side to avoid ever hitting it, just throw on `429`, or something configurable?

## Answer

Configurable, via a new `OpenF1Config` object. `OpenF1` constructor gains an optional `OpenF1Config? config = null` parameter (defaults to `new OpenF1Config()`). `OpenF1Config.UseRateLimit` (`bool`, default `true`) controls whether a built-in client-side pacer (e.g. a semaphore/token-bucket capping outgoing requests at 3/second) sits in front of every HTTP call.

- `UseRateLimit = true` (default): pacer proactively spaces requests so `429` shouldn't normally happen.
- `UseRateLimit = false`: no pacing, caller manages their own request rate.

Either way, `OpenF1RateLimitExceededException` (see [03-error-handling-model](03-error-handling-model.md)) is thrown if the API ever actually returns `429` — the pacer reduces the odds, it doesn't replace the exception. `OpenF1Config` is the extensibility point for future per-instance settings beyond rate limiting (base URL override, timeout, etc. — not decided yet, add as needed).

## Comments
