using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1
{
    /// <summary>Information about individual stints. A stint is a period of continuous driving by a driver during a session.</summary>
    public StintsQuery GetStintsAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<Stint>("stints", qs, c), ct);
}
