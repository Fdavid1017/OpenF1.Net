using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1Client
{
    /// <summary>Real-time interval data between drivers and their gap to the race leader. Available during races only.</summary>
    public IntervalsQuery GetIntervalsAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<Interval>("intervals", qs, c), ct);
}
