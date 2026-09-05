namespace OpenF1.Net.Models;

/// <summary>Championship standings for a team. Only available for race sessions.</summary>
public class ChampionshipTeam
{
    /// <summary>
    /// URL of a left-facing render of the team's current car. Resolved by probing F1's own image assets;
    /// null if no matching asset was found.
    /// </summary>
    public string? CarLeftUrl { get; internal set; }
    /// <summary>
    /// URL of a right-facing render of the team's current car. Resolved by probing F1's own image assets;
    /// null if no matching asset was found.
    /// </summary>
    public string? CarRightUrl { get; internal set; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>Championship points during/after the race (depends on call timing).</summary>
    public double PointsCurrent { get; init; }
    /// <summary>Championship points before the race started.</summary>
    public double PointsStart { get; init; }
    /// <summary>Championship position during/after the race (depends on call timing).</summary>
    public int PositionCurrent { get; init; }
    /// <summary>Championship position before the race started.</summary>
    public int PositionStart { get; init; }
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
    /// <summary>The name of the team.</summary>
    public string TeamName { get; init; } = "";
}
