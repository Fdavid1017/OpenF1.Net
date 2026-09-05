using System.Text.RegularExpressions;

namespace OpenF1.Net.Internal;

/// <summary>Converts an OpenF1Client team name to the slug F1's own image asset paths use (e.g. "Red Bull Racing" -> "redbullracing").</summary>
internal static class TeamNameSlug
{
    static readonly Dictionary<string, string> Overrides = new(StringComparer.Ordinal) { ["Haas F1 Team"] = "haas" };

    static readonly Regex NonAlphanumeric = new("[^a-z0-9]", RegexOptions.Compiled);

    public static string From(string teamName) => Overrides.TryGetValue(teamName, out var slug) ? slug : NonAlphanumeric.Replace(teamName.ToLowerInvariant(), "");
}
