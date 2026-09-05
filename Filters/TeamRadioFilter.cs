using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class TeamRadioFilterFields
{
    public DateTime Date => throw new NotSupportedException();
    public int DriverNumber => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
}

public class TeamRadioQuery(
    Func<string, CancellationToken, Task<TeamRadioMessage[]>> execute,
    Func<int, int, bool, CancellationToken, Task<Driver?>> fetchDriverDetails,
    CancellationToken ct
) : DriverEnrichableQuery<TeamRadioFilterFields, TeamRadioMessage>(execute, fetchDriverDetails, t => t.SessionKey, t => t.DriverNumber, (t, d) => t.DriverDetails = d, ct);
