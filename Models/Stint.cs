using OpenF1.Net.Models.Enums;

namespace OpenF1.Net.Models;

/// <summary>Information about an individual stint. A stint is a period of continuous driving by a driver during a session.</summary>
public class Stint
{
    /// <summary>The specific compound of tyre used during the stint.</summary>
    public TyreCompound Compound { get; init; }
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public int DriverNumber { get; init; }
    /// <summary>The driver this stint belongs to. Null unless the query was built with .IncludeDriverDetails().</summary>
    public Driver? DriverDetails { get; internal set; }
    /// <summary>Number of the last completed lap in this stint. Null while the stint is still in progress.</summary>
    public int? LapEnd { get; init; }
    /// <summary>Number of the initial lap in this stint (starts at 1). Observed null on live data despite the docs implying it's always set.</summary>
    public int? LapStart { get; init; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
    /// <summary>The sequential number of the stint within the session (starts at 1).</summary>
    public int StintNumber { get; init; }
    /// <summary>The age of the tyres at the start of the stint, in laps completed.</summary>
    public int TyreAgeAtStart { get; init; }
}
