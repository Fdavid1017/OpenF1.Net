using System.Text.Json.Serialization;

namespace OpenF1.Net.Models;

/// <summary>Real-time interval data between drivers and their gap to the race leader. Available during races only.</summary>
public class Interval
{
    /// <summary>The UTC date and time, in ISO 8601 format.</summary>
    public DateTime Date { get; init; }
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public int DriverNumber { get; init; }
    /// <summary>The driver this interval belongs to. Null unless the query was built with .IncludeDriverDetails().</summary>
    public Driver? DriverDetails { get; internal set; }
    /// <summary>The time gap to the race leader in seconds, "+1 LAP" if lapped, or null for the race leader.</summary>
    public GapToLeader? GapToLeader { get; init; }
    /// <summary>The time gap to the car ahead in seconds, "+1 LAP" if lapped, or null for the race leader.</summary>
    [JsonPropertyName("interval")]
    public GapToLeader? IntervalValue { get; init; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
}
