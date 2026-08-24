namespace OpenF1.Net.Models;

/// <summary>The approximate location of a car on the circuit, at a sample rate of about 3.7 Hz.</summary>
public class LocationPoint
{
    /// <summary>The UTC date and time, in ISO 8601 format.</summary>
    public DateTime Date { get; init; }
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public int DriverNumber { get; init; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
    /// <summary>The 'x' value in a 3D Cartesian coordinate system representing the current approximate location of the car on the track.</summary>
    public int X { get; init; }
    /// <summary>The 'y' value in a 3D Cartesian coordinate system representing the current approximate location of the car on the track.</summary>
    public int Y { get; init; }
    /// <summary>The 'z' value in a 3D Cartesian coordinate system representing the current approximate location of the car on the track.</summary>
    public int Z { get; init; }
}
