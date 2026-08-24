using System.Net;

namespace OpenF1.Net.Exceptions;

/// <summary>Generic non-2xx catch-all, e.g. the API's 422 "too much data at once" response.</summary>
public class OpenF1ApiException(HttpStatusCode statusCode, string detail) : OpenF1Exception(statusCode, detail);
