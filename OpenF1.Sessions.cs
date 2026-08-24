using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1
{
    /// <summary>
    /// Information about sessions. A session refers to a distinct period of track activity during a
    /// Grand Prix or testing weekend (practice, qualifying, sprint, race, ...). Updated every day at midnight UTC.
    /// </summary>
    public SessionsQuery GetSessionsAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<Session>("sessions", qs, c), ct);
}
