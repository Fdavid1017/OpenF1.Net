using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class SessionResultFilterFields
{
    public bool Dnf => throw new NotSupportedException();
    public bool Dns => throw new NotSupportedException();
    public bool Dsq => throw new NotSupportedException();
    public int DriverNumber => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public int NumberOfLaps => throw new NotSupportedException();
    public int Position => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
    // duration/gap_to_leader intentionally absent — polymorphic wrapper types aren't filterable
}

public class SessionResultQuery(Func<string, CancellationToken, Task<SessionResult[]>> execute, CancellationToken ct)
    : EndpointQuery<SessionResultFilterFields, SessionResult>(execute, ct);
