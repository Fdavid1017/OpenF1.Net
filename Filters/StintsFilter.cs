using OpenF1.Net.Models;
using OpenF1.Net.Models.Enums;

namespace OpenF1.Net.Filters;

public class StintsFilterFields
{
    public TyreCompound Compound => throw new NotSupportedException();
    public int DriverNumber => throw new NotSupportedException();
    public int LapEnd => throw new NotSupportedException();
    public int LapStart => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
    public int StintNumber => throw new NotSupportedException();
    public int TyreAgeAtStart => throw new NotSupportedException();
}

public class StintsQuery(
    Func<string, CancellationToken, Task<Stint[]>> execute,
    Func<int, int, bool, CancellationToken, Task<Driver?>> fetchDriverDetails,
    CancellationToken ct
) : DriverEnrichableQuery<StintsFilterFields, Stint>(execute, fetchDriverDetails, s => s.SessionKey, s => s.DriverNumber, (s, d) => s.DriverDetails = d, ct);
