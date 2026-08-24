using OpenF1.Net.Models;
using OpenF1.Net.Models.Enums;

namespace OpenF1.Net.Filters;

public class SessionsFilterFields
{
    public int CircuitKey => throw new NotSupportedException();
    public string CircuitShortName => throw new NotSupportedException();
    public string CountryCode => throw new NotSupportedException();
    public int CountryKey => throw new NotSupportedException();
    public string CountryName => throw new NotSupportedException();
    public DateTime DateEnd => throw new NotSupportedException();
    public DateTime DateStart => throw new NotSupportedException();
    public bool IsCancelled => throw new NotSupportedException();
    public string Location => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
    public SessionName SessionName => throw new NotSupportedException();
    public SessionType SessionType => throw new NotSupportedException();
    public int Year => throw new NotSupportedException();
}

public class SessionsQuery(Func<string, CancellationToken, Task<Session[]>> execute, CancellationToken ct)
    : EndpointQuery<SessionsFilterFields, Session>(execute, ct);
