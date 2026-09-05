using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenF1.Net.Internal;

/// <summary>
/// Resolves official F1 driver headshot and full-body image URLs without downloading the images
/// themselves — only HEAD requests are sent to confirm an asset exists. Ported from
/// https://github.com/multiviewer/f1-headshots, which discovers these URLs by probing the same
/// per-year, per-driver-reference asset naming scheme F1's own site and graphics use.
/// </summary>
internal static class DriverImageResolver
{
    const string HeadshotTemplateUrl = "https://media.formula1.com/content/dam/fom-website/2018-redesign-assets/drivers/{year}/{ref}.png";
    const string HeadshotFallbackTemplateUrl = "https://assets.multiviewer.dev/driver-headshots/{year}/{tla}.png";
    const string FullBodyTemplateUrl = "https://media.formula1.com/image/upload/c_fill,w_720/q_auto/v1/common/f1/{year}/{team}/{ref}/{year}{team}{ref}{side}.webp";

    const int MinYear = 2017;

    static readonly Dictionary<string, string> TeamSlugOverrides = new(StringComparer.Ordinal)
    {
        ["Haas F1 Team"] = "haas",
    };

    static readonly Regex NonAlphanumeric = new("[^a-z0-9]", RegexOptions.Compiled);

    /// <summary>
    /// Finds the highest-resolution official headshot available for the driver: F1's own asset is tried
    /// across years newest-first (it's the higher-resolution source), then MultiViewer's mirror is tried
    /// the same way. Returns null if neither source has anything for this driver.
    /// </summary>
    public static async Task<string?> ResolveHeadshotUrlAsync(
        HttpClient httpClient,
        string firstName,
        string lastName,
        string nameAcronym,
        CancellationToken ct
    )
    {
        var reference = BuildReference(firstName, lastName);
        var tla = nameAcronym.ToUpperInvariant();

        foreach (var year in CandidateYearsDescending())
        {
            var url = HeadshotTemplateUrl.Replace("{year}", year.ToString(CultureInfo.InvariantCulture)).Replace("{ref}", reference);
            if (await UrlExistsAsync(httpClient, url, ct).ConfigureAwait(false))
                return url;
        }

        foreach (var year in CandidateYearsDescending())
        {
            var url = HeadshotFallbackTemplateUrl.Replace("{year}", year.ToString(CultureInfo.InvariantCulture)).Replace("{tla}", tla);
            if (await UrlExistsAsync(httpClient, url, ct).ConfigureAwait(false))
                return url;
        }

        return null;
    }

    /// <summary>
    /// Finds left- and right-facing full-body renders of the driver in their current team's livery,
    /// trying years newest-first for each side independently. Either side (or both) may come back null
    /// if no matching asset exists.
    /// </summary>
    public static async Task<(string? Left, string? Right)> ResolveFullBodyUrlsAsync(
        HttpClient httpClient,
        string firstName,
        string lastName,
        string teamName,
        CancellationToken ct
    )
    {
        var reference = BuildReference(firstName, lastName);
        var teamSlug = TeamNameToSlug(teamName);

        var left = await ResolveFullBodySideAsync(httpClient, reference, teamSlug, "left", ct).ConfigureAwait(false);
        var right = await ResolveFullBodySideAsync(httpClient, reference, teamSlug, "right", ct).ConfigureAwait(false);
        return (left, right);
    }

    static async Task<string?> ResolveFullBodySideAsync(HttpClient httpClient, string reference, string teamSlug, string side, CancellationToken ct)
    {
        foreach (var year in CandidateYearsDescending())
        {
            var url = FullBodyTemplateUrl
                .Replace("{year}", year.ToString(CultureInfo.InvariantCulture))
                .Replace("{team}", teamSlug)
                .Replace("{ref}", reference.ToLowerInvariant())
                .Replace("{side}", side);
            if (await UrlExistsAsync(httpClient, url, ct).ConfigureAwait(false))
                return url;
        }
        return null;
    }

    // Same reference format F1's own asset paths use: first 3 letters of the first name + first 3 of the
    // last name + "01", e.g. "Max Verstappen" -> "MAXVER01".
    static string BuildReference(string firstName, string lastName)
    {
        var first = RemoveDiacritics(firstName).ToUpperInvariant();
        var last = RemoveDiacritics(lastName).ToUpperInvariant();
        return Take3(first) + Take3(last) + "01";
    }

    static string Take3(string value) => value.Length <= 3 ? value : value[..3];

    static string RemoveDiacritics(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                builder.Append(c);
        }
        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    static string TeamNameToSlug(string teamName) =>
        TeamSlugOverrides.TryGetValue(teamName, out var slug) ? slug : NonAlphanumeric.Replace(teamName.ToLowerInvariant(), "");

    static IEnumerable<int> CandidateYearsDescending()
    {
        for (var year = DateTime.UtcNow.Year + 1; year >= MinYear; year--)
            yield return year;
    }

    static async Task<bool> UrlExistsAsync(HttpClient httpClient, string url, CancellationToken ct)
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
}
