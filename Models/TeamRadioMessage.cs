namespace OpenF1.Net.Models;

/// <summary>A radio exchange between an F1 driver and their team during a session. Only a limited selection is included.</summary>
public class TeamRadioMessage
{
    /// <summary>The UTC date and time, in ISO 8601 format.</summary>
    public DateTime Date { get; init; }
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public int DriverNumber { get; init; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>URL of the radio recording.</summary>
    public string RecordingUrl { get; init; } = "";
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
}
