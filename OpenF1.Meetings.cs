using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1
{
    /// <summary>
    /// Information about meetings. A meeting refers to a Grand Prix or testing weekend and usually
    /// includes multiple sessions (practice, qualifying, race, ...). Meetings are updated every day at midnight UTC.
    /// </summary>
    public MeetingsQuery GetMeetingsAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<Meeting>("meetings", qs, c), ct);
}
