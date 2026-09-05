using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class ChampionshipTeamsFilterFields
{
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public double PointsCurrent => throw new NotSupportedException();
    public double PointsStart => throw new NotSupportedException();
    public int PositionCurrent => throw new NotSupportedException();
    public int PositionStart => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
    public string TeamName => throw new NotSupportedException();
}

public class ChampionshipTeamsQuery(
    Func<string, CancellationToken, Task<ChampionshipTeam[]>> execute,
    Func<ChampionshipTeam, CancellationToken, Task> resolveCarImages,
    CancellationToken ct
) : EndpointQuery<ChampionshipTeamsFilterFields, ChampionshipTeam>(execute, ct)
{
    protected override async Task<ChampionshipTeam[]> ExecuteAsync()
    {
        var teams = await base.ExecuteAsync().ConfigureAwait(false);

        foreach (var team in teams)
            await resolveCarImages(team, CancellationToken).ConfigureAwait(false);

        return teams;
    }
}
