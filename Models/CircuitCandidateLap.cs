namespace OpenF1.Net.Models;

/// <summary>The reference lap the corner/marker positions and track outline on <see cref="CircuitInfo"/> were derived from.</summary>
public class CircuitCandidateLap
{
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public string DriverNumber { get; init; } = "";
    /// <summary>The lap number within the reference session.</summary>
    public int LapNumber { get; init; }
    /// <summary>The UTC date and time the reference lap started.</summary>
    public DateTime LapStartDate { get; init; }
    /// <summary>The session time, in seconds, at which the reference lap started.</summary>
    public double LapStartSessionTime { get; init; }
    /// <summary>The reference lap's duration, in seconds.</summary>
    public double LapTime { get; init; }
    /// <summary>
    /// The session code the reference lap was recorded in (e.g. "FP1"). This is a MultiViewer code, not
    /// the same value set as <see cref="Enums.SessionName"/>, so it's left as a plain string.
    /// </summary>
    public string Session { get; init; } = "";
    /// <summary>The session time, in seconds, at which the reference session started.</summary>
    public double SessionStartTime { get; init; }
}
