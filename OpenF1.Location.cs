using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1
{
    /// <summary>The approximate location of the cars on the circuit, at a sample rate of about 3.7 Hz.</summary>
    public LocationQuery GetLocationAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<LocationPoint>("location", qs, c), ct);
}
