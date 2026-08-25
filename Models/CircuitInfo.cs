namespace OpenF1.Net.Models;

/// <summary>
/// Detailed circuit information (corners, marshal posts, pit lane time loss, official track outline, ...).
/// Populated on a <see cref="Meeting"/> only when the query is built with .IncludeCircuitInfo() — fetched
/// from <see cref="Meeting.CircuitInfoUrl"/>, which is hosted by MultiViewer rather than the OpenF1Client API itself.
/// </summary>
public class CircuitInfo
{
    /// <summary>The unique identifier for the circuit.</summary>
    public int CircuitKey { get; init; }
    /// <summary>The full name of the circuit.</summary>
    public string CircuitName { get; init; } = "";
    /// <summary>The country's IOC (Olympic committee) three-letter code.</summary>
    public string CountryIocCode { get; init; } = "";
    /// <summary>The unique identifier for the country.</summary>
    public int CountryKey { get; init; }
    /// <summary>The full name of the country.</summary>
    public string CountryName { get; init; } = "";
    /// <summary>The city or geographical location of the circuit.</summary>
    public string Location { get; init; } = "";
    /// <summary>The unique identifier for the meeting.</summary>
    public string MeetingKey { get; init; } = "";
    /// <summary>The name of the meeting.</summary>
    public string MeetingName { get; init; } = "";
    /// <summary>The official name of the meeting, when available.</summary>
    public string? MeetingOfficialName { get; init; }
    /// <summary>The estimated time lost driving through the pit lane, under different track conditions.</summary>
    public CircuitPitLoss PitLoss { get; init; } = new();
    /// <summary>The circuit's numbered corners.</summary>
    public CircuitMarker[] Corners { get; init; } = [];
    /// <summary>The marshal light posts around the circuit.</summary>
    public CircuitMarker[] MarshalLights { get; init; } = [];
    /// <summary>The marshal sector boundaries around the circuit.</summary>
    public CircuitMarker[] MarshalSectors { get; init; } = [];
    /// <summary>The reference lap the corner/marker positions and track outline below were derived from.</summary>
    public CircuitCandidateLap? CandidateLap { get; init; }
    /// <summary>Indexes into X/Y marking the boundaries between mini-sectors.</summary>
    public int[] MiniSectorsIndexes { get; init; } = [];
    /// <summary>The date of the race event.</summary>
    public DateTime RaceDate { get; init; }
    /// <summary>The rotation, in degrees, used to orient the track map.</summary>
    public double Rotation { get; init; }
    /// <summary>The championship round number.</summary>
    public int Round { get; init; }
    /// <summary>The 'x' coordinates of the track outline.</summary>
    public double[] X { get; init; } = [];
    /// <summary>The 'y' coordinates of the track outline.</summary>
    public double[] Y { get; init; } = [];
    /// <summary>The year the event takes place.</summary>
    public int Year { get; init; }
}
