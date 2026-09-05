namespace OpenF1.Net.Models;

/// <summary>The starting grid for the upcoming race. Becomes available a few minutes after official results are published.</summary>
public class StartingGridPosition
{
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public int DriverNumber { get; init; }
    /// <summary>The driver holding this grid position. Null unless the query was built with .IncludeDriverDetails().</summary>
    public Driver? DriverDetails { get; internal set; }
    /// <summary>Duration, in seconds, of the qualifying lap.</summary>
    public double? LapDuration { get; init; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>Position on the grid.</summary>
    public int Position { get; init; }
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
}
