using System.Globalization;

namespace OpenF1.Net.Internal;

/// <summary>
/// Resolves official F1 team car render URLs without downloading the images themselves — only HEAD
/// requests are sent to confirm an asset exists.
/// </summary>
internal static class TeamCarImageResolver
{
    const string CarTemplateUrl = "https://media.formula1.com/image/upload/c_lfill,w_3392/q_auto/common/f1/{year}/{team}/{year}{team}car{side}.webp";

    const int MinYear = 2018;

    /// <summary>
    /// Finds left- and right-facing car renders for the team, trying years newest-first for each side
    /// independently (so a team still using last year's livery render doesn't come back empty). Either
    /// side (or both) may come back null if no matching asset exists.
    /// </summary>
    public static async Task<(string? Left, string? Right)> ResolveCarUrlsAsync(HttpClient httpClient, string teamName, CancellationToken ct)
    {
        var teamSlug = TeamNameSlug.From(teamName);

        var left = await ResolveSideAsync(httpClient, teamSlug, "left", ct).ConfigureAwait(false);
        var right = await ResolveSideAsync(httpClient, teamSlug, "right", ct).ConfigureAwait(false);
        return (left, right);
    }

    static async Task<string?> ResolveSideAsync(HttpClient httpClient, string teamSlug, string side, CancellationToken ct)
    {
        foreach (var year in ImageAssetProbe.CandidateYearsDescending(MinYear))
        {
            var url = CarTemplateUrl.Replace("{year}", year.ToString(CultureInfo.InvariantCulture)).Replace("{team}", teamSlug).Replace("{side}", side);
            if (await ImageAssetProbe.ExistsAsync(httpClient, url, ct).ConfigureAwait(false))
                return url;
        }
        return null;
    }
}
