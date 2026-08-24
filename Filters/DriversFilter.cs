using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class DriversFilterFields
{
    public string BroadcastName => throw new NotSupportedException();
    public int DriverNumber => throw new NotSupportedException();
    public string FirstName => throw new NotSupportedException();
    public string FullName => throw new NotSupportedException();
    public string LastName => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public string NameAcronym => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
    public string TeamName => throw new NotSupportedException();
}

public class DriversQuery(Func<string, CancellationToken, Task<Driver[]>> execute, CancellationToken ct)
    : EndpointQuery<DriversFilterFields, Driver>(execute, ct);
