using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class IntervalsFilterFields
{
    public DateTime Date => throw new NotSupportedException();
    public int DriverNumber => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
    // gap_to_leader/interval intentionally absent — polymorphic wrapper types aren't filterable
}

public class IntervalsQuery(Func<string, CancellationToken, Task<Interval[]>> execute, CancellationToken ct)
    : EndpointQuery<IntervalsFilterFields, Interval>(execute, ct);
