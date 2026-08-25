using System.Text.Json;
using System.Text.Json.Serialization;
using OpenF1.Net.Filters;
using OpenF1.Net.Internal;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1Client
{
    /// <summary>
    /// Information about meetings. A meeting refers to a Grand Prix or testing weekend and usually
    /// includes multiple sessions (practice, qualifying, race, ...). Meetings are updated every day at midnight UTC.
    /// </summary>
    public MeetingsQuery GetMeetingsAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<Meeting>("meetings", qs, c), FetchCircuitInfoAsync, ct);

    // MultiViewer's JSON is camelCase and returns a single object (not an array, unlike every openf1.org
    // endpoint), and some numeric fields (e.g. pitLoss) arrive as quoted strings — hence a dedicated
    // options instance rather than reusing the shared snake_case JsonOptions.
    static readonly JsonSerializerOptions CircuitInfoJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new UtcDateTimeConverter() },
    };

    /// <summary>
    /// Fetches circuit info from a meeting's CircuitInfoUrl. Hosted by MultiViewer, not the OpenF1Client API —
    /// no rate limiting or openf1 error-detail parsing applies here.
    /// </summary>
    internal async Task<CircuitInfo> FetchCircuitInfoAsync(string url, CancellationToken ct)
    {
        using var response = await _httpClient.GetAsync(url, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        return await JsonSerializer.DeserializeAsync<CircuitInfo>(stream, CircuitInfoJsonOptions, ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Empty circuit info response from {url}.");
    }
}
