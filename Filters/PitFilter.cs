using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class PitFilterFields
{
    public DateTime Date => throw new NotSupportedException();
    public int DriverNumber => throw new NotSupportedException();
    public double LaneDuration => throw new NotSupportedException();
    public int LapNumber => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
    public double StopDuration => throw new NotSupportedException();
}

public class PitQuery(Func<string, CancellationToken, Task<PitStop[]>> execute, CancellationToken ct)
    : EndpointQuery<PitFilterFields, PitStop>(execute, ct);
