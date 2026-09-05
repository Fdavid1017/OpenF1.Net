namespace OpenF1.Net.Internal;

/// <summary>Shared HEAD-request existence check used by the image resolvers — never downloads the asset itself.</summary>
internal static class ImageAssetProbe
{
    public static async Task<bool> ExistsAsync(HttpClient httpClient, string url, CancellationToken ct)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, url);
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    /// <summary>Years to probe, newest first — from next year (assets sometimes appear ahead of the season) down to <paramref name="minYear"/>.</summary>
    public static IEnumerable<int> CandidateYearsDescending(int minYear)
    {
        for (var year = DateTime.UtcNow.Year + 1; year >= minYear; year--)
            yield return year;
    }
}
