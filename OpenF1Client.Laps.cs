using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1Client
{
    /// <summary>Detailed information about individual laps.</summary>
    public LapsQuery GetLapsAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<Lap>("laps", qs, c), FetchDriverDetailsAsync, ct);
}
