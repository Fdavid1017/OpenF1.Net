using System.Net;

namespace OpenF1.Net.Exceptions;

/// <summary>Base type for every exception OpenF1.Net throws for a non-success API response.</summary>
public abstract class OpenF1Exception(HttpStatusCode statusCode, string detail) : Exception(detail)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public string Detail { get; } = detail;
}
