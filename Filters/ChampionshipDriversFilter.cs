using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class ChampionshipDriversFilterFields
{
    public int DriverNumber => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public double PointsCurrent => throw new NotSupportedException();
    public double PointsStart => throw new NotSupportedException();
    public int PositionCurrent => throw new NotSupportedException();
    public int PositionStart => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
}

public class ChampionshipDriversQuery(
    Func<string, CancellationToken, Task<ChampionshipDriver[]>> execute,
    Func<int, int, bool, CancellationToken, Task<Driver?>> fetchDriverDetails,
    CancellationToken ct
) : DriverEnrichableQuery<ChampionshipDriversFilterFields, ChampionshipDriver>(execute, fetchDriverDetails, c => c.SessionKey, c => c.DriverNumber, (c, d) => c.DriverDetails = d, ct);
