using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class PositionFilterFields
{
    public DateTime Date => throw new NotSupportedException();
    public int DriverNumber => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public int Position => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
}

public class PositionQuery(Func<string, CancellationToken, Task<Position[]>> execute, CancellationToken ct)
    : EndpointQuery<PositionFilterFields, Position>(execute, ct);
