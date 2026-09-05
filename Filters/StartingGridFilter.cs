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

public class StartingGridQuery(
    Func<string, CancellationToken, Task<StartingGridPosition[]>> execute,
    Func<int, int, bool, CancellationToken, Task<Driver?>> fetchDriverDetails,
    CancellationToken ct
) : DriverEnrichableQuery<StartingGridFilterFields, StartingGridPosition>(execute, fetchDriverDetails, g => g.SessionKey, g => g.DriverNumber, (g, d) => g.DriverDetails = d, ct);
