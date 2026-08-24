using OpenF1.Net.Models.Enums;

namespace OpenF1.Net.Models;

/// <summary>
/// A session refers to a distinct period of track activity during a Grand Prix or testing weekend
/// (practice, qualifying, sprint, race, ...). Sessions are updated every day at midnight UTC.
/// </summary>
public class Session
{
    /// <summary>The unique identifier for the circuit where the event takes place.</summary>
    public int CircuitKey { get; init; }
    /// <summary>The short or common name of the circuit where the event takes place.</summary>
    public string CircuitShortName { get; init; } = "";
    /// <summary>A code that uniquely identifies the country.</summary>
    public string CountryCode { get; init; } = "";
    /// <summary>The unique identifier for the country where the event takes place.</summary>
    public int CountryKey { get; init; }
    /// <summary>The full name of the country where the event takes place.</summary>
    public string CountryName { get; init; } = "";
    /// <summary>The UTC ending date and time, in ISO 8601 format.</summary>
    public DateTime DateEnd { get; init; }
    /// <summary>The UTC starting date and time, in ISO 8601 format.</summary>
    public DateTime DateStart { get; init; }
    /// <summary>The difference in hours and minutes between local time at the location of the event and Greenwich Mean Time (GMT).</summary>
    public string GmtOffset { get; init; } = "";
    /// <summary>A boolean indicating whether the session has been cancelled.</summary>
    public bool IsCancelled { get; init; }
    /// <summary>The city or geographical location where the event takes place.</summary>
    public string Location { get; init; } = "";
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
    /// <summary>The name of the session (Practice 1, Qualifying, Race, ...).</summary>
    public SessionName SessionName { get; init; }
    /// <summary>The type of the session (Practice, Qualifying, Race, ...).</summary>
    public SessionType SessionType { get; init; }
    /// <summary>The year the event takes place.</summary>
    public int Year { get; init; }
}
