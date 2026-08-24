using OpenF1.Net.Models;
using OpenF1.Net.Models.Enums;

namespace OpenF1.Net.Filters;

public class RaceControlFilterFields
{
    public Category Category => throw new NotSupportedException();
    public DateTime Date => throw new NotSupportedException();
    public int DriverNumber => throw new NotSupportedException();
    public Flag Flag => throw new NotSupportedException();
    public int LapNumber => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public int QualifyingPhase => throw new NotSupportedException();
    public Scope Scope => throw new NotSupportedException();
    public int Sector => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
}

public class RaceControlQuery(Func<string, CancellationToken, Task<RaceControlMessage[]>> execute, CancellationToken ct)
    : EndpointQuery<RaceControlFilterFields, RaceControlMessage>(execute, ct);
