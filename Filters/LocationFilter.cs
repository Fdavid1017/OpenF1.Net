using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class LocationFilterFields
{
    public DateTime Date => throw new NotSupportedException();
    public int DriverNumber => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
    public int X => throw new NotSupportedException();
    public int Y => throw new NotSupportedException();
    public int Z => throw new NotSupportedException();
}

public class LocationQuery(
    Func<string, CancellationToken, Task<LocationPoint[]>> execute,
    Func<int, int, bool, CancellationToken, Task<Driver?>> fetchDriverDetails,
    CancellationToken ct
) : DriverEnrichableQuery<LocationFilterFields, LocationPoint>(execute, fetchDriverDetails, l => l.SessionKey, l => l.DriverNumber, (l, d) => l.DriverDetails = d, ct);
