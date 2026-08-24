using System.Text.Json.Serialization;

namespace OpenF1.Net.Models;

/// <summary>Driver position throughout a session, including initial placement and subsequent changes.</summary>
public class Position
{
    /// <summary>The UTC date and time, in ISO 8601 format.</summary>
    public DateTime Date { get; init; }
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public int DriverNumber { get; init; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>Position of the driver (starts at 1).</summary>
    [JsonPropertyName("position")]
    public int PositionValue { get; init; }
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
}
