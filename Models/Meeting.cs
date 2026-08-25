using OpenF1.Net.Models.Enums;

namespace OpenF1.Net.Models;

/// <summary>
/// A meeting refers to a Grand Prix or testing weekend and usually includes multiple sessions
/// (practice, qualifying, race, ...). Meetings are updated every day at midnight UTC.
/// </summary>
public class Meeting
{
    /// <summary>The unique identifier for the circuit where the event takes place.</summary>
    public int CircuitKey { get; init; }
    /// <summary>A URL to a JSON containing detailed circuit info. See FastF1 documentation for details. Data provided by MultiViewer.</summary>
    public string CircuitInfoUrl { get; init; } = "";
    /// <summary>
    /// Detailed circuit information (corners, marshal posts, pit lane loss, track outline, ...), fetched
    /// from <see cref="CircuitInfoUrl"/>. Null unless the query was built with .IncludeCircuitInfo().
    /// </summary>
    public CircuitInfo? CircuitInfo { get; internal set; }
    /// <summary>The short or common name of the circuit where the event takes place.</summary>
    public string CircuitShortName { get; init; } = "";
    /// <summary>The type of the circuit ("Permanent" or "Temporary - Street/Road").</summary>
    public CircuitType CircuitType { get; init; }
    /// <summary>A code that uniquely identifies the country.</summary>
    public string CountryCode { get; init; } = "";
    /// <summary>An image of the country flag.</summary>
    public string CountryFlag { get; init; } = "";
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
    /// <summary>A boolean indicating whether the meeting has been cancelled.</summary>
    public bool IsCancelled { get; init; }
    /// <summary>The city or geographical location where the event takes place.</summary>
    public string Location { get; init; } = "";
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>The name of the meeting.</summary>
    public string MeetingName { get; init; } = "";
    /// <summary>The official name of the meeting.</summary>
    public string MeetingOfficialName { get; init; } = "";
    /// <summary>The year the event takes place.</summary>
    public int Year { get; init; }
}
