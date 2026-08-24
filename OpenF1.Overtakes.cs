using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1
{
    /// <summary>
    /// Information about overtakes. An overtake refers to one driver exchanging positions with another,
    /// including on-track passes and position changes from pit stops or post-race penalties.
    /// Available during races only, and may be incomplete.
    /// </summary>
    public OvertakesQuery GetOvertakesAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<Overtake>("overtakes", qs, c), ct);
}
