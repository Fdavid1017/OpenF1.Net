using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class StartingGridFilterFields
{
    public int DriverNumber => throw new NotSupportedException();
    public double LapDuration => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public int Position => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
}

public class StartingGridQuery(Func<string, CancellationToken, Task<StartingGridPosition[]>> execute, CancellationToken ct)
    : EndpointQuery<StartingGridFilterFields, StartingGridPosition>(execute, ct);
