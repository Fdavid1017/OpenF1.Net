namespace OpenF1.Net.Models;

/// <summary>Standings after a session. Becomes available a few minutes after official results are published.</summary>
public class SessionResult
{
    /// <summary>Indicates whether the driver Did Not Finish the race. Can be true only for qualifying and race sessions.</summary>
    public bool Dnf { get; init; }
    /// <summary>Indicates whether the driver Did Not Start the race. Can be true only for qualifying and race sessions.</summary>
    public bool Dns { get; init; }
    /// <summary>Indicates whether the driver was disqualified.</summary>
    public bool Dsq { get; init; }
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public int DriverNumber { get; init; }
    /// <summary>The driver this result belongs to. Null unless the query was built with .IncludeDriverDetails().</summary>
    public Driver? DriverDetails { get; internal set; }
    /// <summary>
    /// Either the best lap time (for practice or qualifying), or the total race time (for races), in
    /// seconds. In qualifying, this holds three values for Q1, Q2, and Q3 instead.
    /// </summary>
    public SessionResultDuration? Duration { get; init; }
    /// <summary>
    /// The time gap to the session leader in seconds, or "+N LAP(S)" if the driver was lapped. In
    /// qualifying, this holds three values for Q1, Q2, and Q3 instead.
    /// </summary>
    public SessionResultGapToLeader? GapToLeader { get; init; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>Total number of laps completed during the session.</summary>
    public int? NumberOfLaps { get; init; }
    /// <summary>The driver's final position at the end of the session.</summary>
    public int? Position { get; init; }
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
}
