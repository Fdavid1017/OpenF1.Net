namespace OpenF1.Net.Models;

/// <summary>
/// A labeled point along the circuit outline — corners, marshal light posts, and marshal sector
/// boundaries all share this shape.
/// </summary>
public class CircuitMarker
{
    /// <summary>The marker's number, in track order.</summary>
    public int Number { get; init; }
    /// <summary>The angle, in degrees, at which the marker's label should be drawn relative to the track.</summary>
    public double Angle { get; init; }
    /// <summary>The distance, in meters, along the track outline from the start line to this marker.</summary>
    public double Length { get; init; }
    /// <summary>The marker's position in the circuit's local coordinate system.</summary>
    public CircuitPosition TrackPosition { get; init; } = new();
}
