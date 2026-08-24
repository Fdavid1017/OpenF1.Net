using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class TeamRadioFilterFields
{
    public DateTime Date => throw new NotSupportedException();
    public int DriverNumber => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
}

public class TeamRadioQuery(Func<string, CancellationToken, Task<TeamRadioMessage[]>> execute, CancellationToken ct)
    : EndpointQuery<TeamRadioFilterFields, TeamRadioMessage>(execute, ct);
