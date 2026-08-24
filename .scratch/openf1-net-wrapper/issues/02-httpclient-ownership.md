Type: grilling
Status: resolved

## Question

How does the `OpenF1` class obtain its `HttpClient` — does it always create its own internally, or can a consumer inject one (important for a NuGet library used inside DI-managed apps)?

## Answer

Optional injection. Constructor signature includes `HttpClient? httpClient = null`. If `null`, `OpenF1` creates its own internally and owns its lifetime (`IDisposable`/`IAsyncDisposable` on `OpenF1`, disposing only the client it created itself — never dispose a caller-supplied one). This makes `new OpenF1()` work standalone and `new OpenF1(myFactory.CreateClient())` work in DI apps.

## Comments
