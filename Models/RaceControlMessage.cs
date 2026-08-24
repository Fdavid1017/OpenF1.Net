using OpenF1.Net.Models.Enums;

namespace OpenF1.Net.Models;

/// <summary>Information about race control (session status, racing incidents, flags, safety car, ...).</summary>
public class RaceControlMessage
{
    /// <summary>The category of the event (SessionStatus, CarEvent, Drs, Flag, SafetyCar, ...).</summary>
    public Category Category { get; init; }
    /// <summary>The UTC date and time, in ISO 8601 format.</summary>
    public DateTime Date { get; init; }
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public int? DriverNumber { get; init; }
    /// <summary>Type of flag displayed (GREEN, YELLOW, DOUBLE YELLOW, CHEQUERED, ...).</summary>
    public Flag? Flag { get; init; }
    /// <summary>The sequential number of the lap within the session (starts at 1), in a race.</summary>
    public int? LapNumber { get; init; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>Description of the event or action.</summary>
    public string Message { get; init; } = "";
    /// <summary>The specific phase (1, 2, or 3) if the session is a qualifying session.</summary>
    public int? QualifyingPhase { get; init; }
    /// <summary>The scope of the event (Track, Driver, Sector, ...).</summary>
    public Scope? Scope { get; init; }
    /// <summary>Segment ("mini-sector") of the track where the event occurred (starts at 1).</summary>
    public int? Sector { get; init; }
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
}
