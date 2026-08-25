namespace OpenF1.Net.Models;

/// <summary>Estimated time lost, in seconds, driving through the pit lane, under different track conditions.</summary>
public class CircuitPitLoss
{
    /// <summary>Time lost under green-flag racing conditions.</summary>
    public decimal Normal { get; init; }
    /// <summary>Time lost under Safety Car conditions.</summary>
    public decimal Sc { get; init; }
    /// <summary>Time lost under Virtual Safety Car conditions.</summary>
    public decimal Vsc { get; init; }
}
