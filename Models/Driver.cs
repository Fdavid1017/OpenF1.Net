namespace OpenF1.Net.Models;

/// <summary>Detailed information about a driver participating in a specific session.</summary>
public class Driver
{
    /// <summary>The driver's name, as displayed on TV.</summary>
    public string BroadcastName { get; init; } = "";
    /// <summary>A code that uniquely identifies the country. This field will be removed at the end of the 2026 season.</summary>
    [Obsolete("Removed at the end of the 2026 season, per the OpenF1 docs.")]
    public string? CountryCode { get; init; }
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public int DriverNumber { get; init; }
    /// <summary>The driver's first name.</summary>
    public string FirstName { get; init; } = "";
    /// <summary>The driver's full name.</summary>
    public string FullName { get; init; } = "";
    /// <summary>URL of the driver's face photo.</summary>
    public string HeadshotUrl { get; init; } = "";
    /// <summary>The driver's last name.</summary>
    public string LastName { get; init; } = "";
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>Three-letter acronym of the driver's name.</summary>
    public string NameAcronym { get; init; } = "";
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
    /// <summary>The hexadecimal color value (RRGGBB) of the driver's team.</summary>
    public string TeamColour { get; init; } = "";
    /// <summary>Name of the driver's team.</summary>
    public string TeamName { get; init; } = "";
}
