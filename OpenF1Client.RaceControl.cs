using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1Client
{
    /// <summary>Information about race control (session status, racing incidents, flags, safety car, ...).</summary>
    public RaceControlQuery GetRaceControlAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<RaceControlMessage>("race_control", qs, c), ct);
}
