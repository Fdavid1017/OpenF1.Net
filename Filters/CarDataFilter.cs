using OpenF1.Net.Models;
using OpenF1.Net.Models.Enums;

namespace OpenF1.Net.Filters;

public class CarDataFilterFields
{
    public int Brake => throw new NotSupportedException();
    public DateTime Date => throw new NotSupportedException();
    public int DriverNumber => throw new NotSupportedException();
    public DrsStatus Drs => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public int NGear => throw new NotSupportedException();
    public int Rpm => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
    public int Speed => throw new NotSupportedException();
    public int Throttle => throw new NotSupportedException();
}

public class CarDataQuery(Func<string, CancellationToken, Task<CarDataPoint[]>> execute, CancellationToken ct)
    : EndpointQuery<CarDataFilterFields, CarDataPoint>(execute, ct);
