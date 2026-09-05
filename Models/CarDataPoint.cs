using OpenF1.Net.Models.Enums;

namespace OpenF1.Net.Models;

/// <summary>Some data about each car, at a sample rate of about 3.7 Hz.</summary>
public class CarDataPoint
{
    /// <summary>Whether the brake pedal is pressed (100) or not (0).</summary>
    public int Brake { get; init; }
    /// <summary>The UTC date and time, in ISO 8601 format.</summary>
    public DateTime Date { get; init; }
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public int DriverNumber { get; init; }
    /// <summary>The driver this data point belongs to. Null unless the query was built with .IncludeDriverDetails().</summary>
    public Driver? DriverDetails { get; internal set; }
    /// <summary>The Drag Reduction System (DRS) status.</summary>
    public DrsStatus Drs { get; init; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>Current gear selection, ranging from 1 to 8. 0 indicates neutral or no gear engaged.</summary>
    public int NGear { get; init; }
    /// <summary>Revolutions per minute of the engine.</summary>
    public int Rpm { get; init; }
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
    /// <summary>Velocity of the car in km/h.</summary>
    public int Speed { get; init; }
    /// <summary>Percentage of maximum engine power being used.</summary>
    public int Throttle { get; init; }
}
