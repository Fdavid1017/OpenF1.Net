using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class OvertakesFilterFields
{
    public DateTime Date => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public int OvertakenDriverNumber => throw new NotSupportedException();
    public int OvertakingDriverNumber => throw new NotSupportedException();
    public int Position => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
}

public class OvertakesQuery(Func<string, CancellationToken, Task<Overtake[]>> execute, CancellationToken ct)
    : EndpointQuery<OvertakesFilterFields, Overtake>(execute, ct);
