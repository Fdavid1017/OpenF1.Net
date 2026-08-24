using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1
{
    /// <summary>Detailed information about the drivers participating in a specific session.</summary>
    public DriversQuery GetDriversAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<Driver>("drivers", qs, c), ct);
}
