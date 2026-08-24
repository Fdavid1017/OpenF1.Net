using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenF1.Net.Exceptions;
using OpenF1.Net.Internal;
using OpenF1.Net.Models;
using OpenF1.Net.Models.Enums;

namespace OpenF1.Net;

/// <summary>.NET wrapper for the OpenF1 API (https://openf1.org/), providing real-time and historical Formula 1 data.</summary>
public partial class OpenF1 : IAsyncDisposable
{
    const string BaseUrl = "https://api.openf1.org/v1";
    const string NoResultsDetail = "No results found.";

    static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        // Flag/Scope route through here (not a [JsonConverter] on the enum) — see the note on Flag.
        Converters = { new UtcDateTimeConverter(), new NullableFlagJsonConverter(), new NullableScopeJsonConverter() },
    };

    readonly HttpClient _httpClient;
    readonly bool _ownsHttpClient;
    readonly OpenF1Config _config;
    readonly ILogger _logger;
    readonly RateLimiter? _rateLimiter;

    public OpenF1(HttpClient? httpClient = null, OpenF1Config? config = null, ILogger? logger = null)
    {
        _ownsHttpClient = httpClient is null;
        _httpClient = httpClient ?? new HttpClient();
        _config = config ?? new OpenF1Config();
        _logger = logger ?? NullLogger.Instance;
        _rateLimiter = _config.UseRateLimit ? new RateLimiter() : null;
    }

    /// <summary>The single current session, resolved via session_key=latest.</summary>
    public async Task<Session?> GetLatestSessionAsync(CancellationToken ct = default)
    {
        var sessions = await ExecuteAsync<Session>("sessions", "session_key=latest", ct).ConfigureAwait(false);
        return sessions.Length > 0 ? sessions[0] : null;
    }

    /// <summary>The single current meeting, resolved via meeting_key=latest.</summary>
    public async Task<Meeting?> GetLatestMeetingAsync(CancellationToken ct = default)
    {
        var meetings = await ExecuteAsync<Meeting>("meetings", "meeting_key=latest", ct).ConfigureAwait(false);
        return meetings.Length > 0 ? meetings[0] : null;
    }

    internal async Task<T[]> ExecuteAsync<T>(string path, string queryString, CancellationToken ct)
    {
        if (_rateLimiter is not null)
            await _rateLimiter.WaitAsync(ct).ConfigureAwait(false);

        var url = string.IsNullOrEmpty(queryString) ? $"{BaseUrl}/{path}" : $"{BaseUrl}/{path}?{queryString}";
        _logger.LogInformation("OpenF1 request: GET {Url}", url);

        using var response = await _httpClient.GetAsync(url, ct).ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync<T[]>(stream, JsonOptions, ct).ConfigureAwait(false) ?? [];
        }

        var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        var detail = TryParseDetail(body) ?? body;

        if (response.StatusCode == HttpStatusCode.NotFound && detail == NoResultsDetail)
            return [];

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            _logger.LogWarning("OpenF1 rate limit exceeded: {Detail}", detail);
            throw new OpenF1RateLimitExceededException(detail);
        }

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            _logger.LogError("OpenF1 request requires a subscription: {StatusCode} {Detail}", response.StatusCode, detail);
            throw new OpenF1SubscriptionRequiredException(response.StatusCode, detail);
        }

        _logger.LogError("OpenF1 request failed: {StatusCode} {Detail}", response.StatusCode, detail);
        throw new OpenF1ApiException(response.StatusCode, detail);
    }

    static string? TryParseDetail(string body)
    {
        try
        {
            return JsonSerializer.Deserialize<ApiErrorResponse>(body, JsonOptions)?.Detail;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public ValueTask DisposeAsync()
    {
        if (_ownsHttpClient)
            _httpClient.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
