using System.Text.Json.Serialization;
using OpenF1.Net.Models.Enums;

namespace OpenF1.Net.Models;

/// <summary>Detailed information about an individual lap.</summary>
public class Lap
{
    /// <summary>The UTC starting date and time, in ISO 8601 format. This date is approximate.</summary>
    public DateTime DateStart { get; init; }
    /// <summary>The unique number assigned to an F1 driver for the season (cf. Wikipedia).</summary>
    public int DriverNumber { get; init; }
    /// <summary>The time taken, in seconds, to complete the first sector of the lap.</summary>
    // SnakeCaseLower's default conversion drops the underscore before a trailing digit (-> "duration_sector1"), so it's spelled out explicitly here.
    [JsonPropertyName("duration_sector_1")]
    public double? DurationSector1 { get; init; }
    /// <summary>The time taken, in seconds, to complete the second sector of the lap.</summary>
    [JsonPropertyName("duration_sector_2")]
    public double? DurationSector2 { get; init; }
    /// <summary>The time taken, in seconds, to complete the third sector of the lap.</summary>
    [JsonPropertyName("duration_sector_3")]
    public double? DurationSector3 { get; init; }
    /// <summary>The speed of the car, in km/h, at the first intermediate point on the track.</summary>
    public int? I1Speed { get; init; }
    /// <summary>The speed of the car, in km/h, at the second intermediate point on the track.</summary>
    public int? I2Speed { get; init; }
    /// <summary>A boolean value indicating whether the lap is an "out" lap from the pit.</summary>
    public bool IsPitOutLap { get; init; }
    /// <summary>The total time taken, in seconds, to complete the entire lap.</summary>
    public double? LapDuration { get; init; }
    /// <summary>The sequential number of the lap within the session (starts at 1).</summary>
    public int LapNumber { get; init; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>A list of values representing the "mini-sectors" within the first sector.</summary>
    [JsonPropertyName("segments_sector_1")]
    public SegmentStatus[] SegmentsSector1 { get; init; } = [];
    /// <summary>A list of values representing the "mini-sectors" within the second sector.</summary>
    [JsonPropertyName("segments_sector_2")]
    public SegmentStatus[] SegmentsSector2 { get; init; } = [];
    /// <summary>A list of values representing the "mini-sectors" within the third sector.</summary>
    [JsonPropertyName("segments_sector_3")]
    public SegmentStatus[] SegmentsSector3 { get; init; } = [];
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
    /// <summary>The speed of the car, in km/h, at the speed trap.</summary>
    public int? StSpeed { get; init; }
}
