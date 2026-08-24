using System.Net;

namespace OpenF1.Net.Exceptions;

/// <summary>The API returned 429 — max 3 requests/second exceeded.</summary>
public class OpenF1RateLimitExceededException(string detail) : OpenF1Exception(HttpStatusCode.TooManyRequests, detail);
