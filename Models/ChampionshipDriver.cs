namespace OpenF1.Net.Models;

/// <summary>Championship standings for a driver. Only available for race sessions.</summary>
public class ChampionshipDriver
{
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public int DriverNumber { get; init; }
    /// <summary>The driver these standings belong to. Null unless the query was built with .IncludeDriverDetails().</summary>
    public Driver? DriverDetails { get; internal set; }
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
}
