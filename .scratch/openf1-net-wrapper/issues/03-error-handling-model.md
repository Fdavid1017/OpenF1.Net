Type: grilling
Status: resolved

## Question

How should OpenF1.Net surface API errors — exception hierarchy or a `Result<T>`-style return wrapper — and how does the "live session without subscription" case (called out explicitly in the original ask) fit in, given what the real API actually returns?

## Answer

Exception hierarchy, not a result wrapper. Methods throw; `ILogger` logs at `Error` before throwing.

Live research against `api.openf1.org` during charting (2026-08-24) found the real error surface differs from what the docs imply — there is no observed distinct "subscription required" response; unauthenticated `session_key=latest` requests returned plain `200`. Three real error shapes were confirmed instead:

- `422` — payload too large: `{"detail":"Failed to retrieve information. You're likely asking for too much data at once."}`
- `404` — no match: `{"detail":"No results found."}` (see [04-not-found-handling](04-not-found-handling.md) — this one does NOT become an exception)
- `429` — rate limit: `{"detail":"Rate limit exceeded. Max 3 requests/second.","error":"Too Many Requests"}` (see [05-rate-limiting-config](05-rate-limiting-config.md))

Hierarchy:

- `OpenF1Exception` (abstract base, carries `HttpStatusCode StatusCode` and `string Detail`)
  - `OpenF1ApiException` — generic non-2xx catch-all (covers the `422` case and anything else unmapped)
  - `OpenF1RateLimitExceededException` — `429`
  - `OpenF1SubscriptionRequiredException` — kept as a defensive placeholder for `401`/`403`, per the docs' stated (but unreproduced live) OAuth2 requirement for real-time data. See [Out of scope](../map.md) — building the actual OAuth2 flow is out of scope; this exception type just means "if the API ever does return 401/403, don't lump it into the generic bucket."

## Comments
