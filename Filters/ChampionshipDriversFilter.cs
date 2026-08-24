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

public class ChampionshipDriversQuery(Func<string, CancellationToken, Task<ChampionshipDriver[]>> execute, CancellationToken ct)
    : EndpointQuery<ChampionshipDriversFilterFields, ChampionshipDriver>(execute, ct);
