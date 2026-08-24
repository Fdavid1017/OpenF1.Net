namespace OpenF1.Net.Models;

/// <summary>Information about a car going through the pit lane.</summary>
public class PitStop
{
    /// <summary>The UTC date and time, in ISO 8601 format.</summary>
    public DateTime Date { get; init; }
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public int DriverNumber { get; init; }
    /// <summary>The time spent in the pit lane, in seconds.</summary>
    public double LaneDuration { get; init; }
    /// <summary>The sequential number of the lap within the session (starts at 1).</summary>
    public int LapNumber { get; init; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>Same as LaneDuration. This field will be removed at the end of the 2026 season.</summary>
    [Obsolete("Same as " + nameof(LaneDuration) + ". Removed at the end of the 2026 season, per the OpenF1 docs.")]
    public double PitDuration { get; init; }
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
    /// <summary>The stationary pit stop time, in seconds. Only available from the 2024 US GP onwards.</summary>
    public double? StopDuration { get; init; }
}
