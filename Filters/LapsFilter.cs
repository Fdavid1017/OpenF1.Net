using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class LapsFilterFields
{
    public DateTime DateStart => throw new NotSupportedException();
    public int DriverNumber => throw new NotSupportedException();
    public double DurationSector1 => throw new NotSupportedException();
    public double DurationSector2 => throw new NotSupportedException();
    public double DurationSector3 => throw new NotSupportedException();
    public int I1Speed => throw new NotSupportedException();
    public int I2Speed => throw new NotSupportedException();
    public bool IsPitOutLap => throw new NotSupportedException();
    public double LapDuration => throw new NotSupportedException();
    public int LapNumber => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
    public int StSpeed => throw new NotSupportedException();
    // segments_sector_1/2/3 intentionally absent — arrays aren't filterable
}

public class LapsQuery(Func<string, CancellationToken, Task<Lap[]>> execute, CancellationToken ct)
    : EndpointQuery<LapsFilterFields, Lap>(execute, ct);
