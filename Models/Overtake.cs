namespace OpenF1.Net.Models;

/// <summary>
/// An overtake refers to one driver exchanging positions with another, including both on-track passes
/// and position changes from pit stops or post-race penalties. Available during races only, may be incomplete.
/// </summary>
public class Overtake
{
    /// <summary>The UTC date and time, in ISO 8601 format.</summary>
    public DateTime Date { get; init; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>The unique number assigned to the overtaken F1 driver (cf. Wikipedia).</summary>
    public int OvertakenDriverNumber { get; init; }
    /// <summary>The unique number assigned to the overtaking F1 driver (cf. Wikipedia).</summary>
    public int OvertakingDriverNumber { get; init; }
    /// <summary>The position of the overtaking F1 driver after the overtake was completed (starts at 1).</summary>
    public int Position { get; init; }
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
}
