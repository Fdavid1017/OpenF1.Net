using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1Client
{
    /// <summary>Standings after a session. Becomes available a few minutes after official results are published.</summary>
    public SessionResultQuery GetSessionResultAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<SessionResult>("session_result", qs, c), FetchDriverDetailsAsync, ct);
}
