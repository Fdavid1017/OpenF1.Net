namespace OpenF1.Net;

/// <summary>Per-instance configuration for <see cref="OpenF1Client"/>.</summary>
public class OpenF1Config
{
    /// <summary>
    /// When true (default), a built-in client-side pacer spaces outgoing requests to stay under the
    /// API's 3 requests/second cap. When false, the caller is responsible for its own request rate.
    /// Either way, a real 429 from the API still throws <see cref="Exceptions.OpenF1RateLimitExceededException"/>.
    /// </summary>
    public bool UseRateLimit { get; init; } = true;
}
