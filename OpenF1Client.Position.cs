using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1Client
{
    /// <summary>Driver positions throughout a session, including initial placement and subsequent changes.</summary>
    public PositionQuery GetPositionAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<Position>("position", qs, c), ct);
}
