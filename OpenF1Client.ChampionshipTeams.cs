using OpenF1.Net.Filters;
using OpenF1.Net.Internal;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1Client
{
    /// <summary>
    /// Championship standings for teams. Only available for race sessions. Each team's
    /// <see cref="ChampionshipTeam.CarLeftUrl"/> / <see cref="ChampionshipTeam.CarRightUrl"/> are always
    /// resolved by probing F1's own car render assets — no image is downloaded, only HEAD requests confirm
    /// each candidate URL exists.
    /// </summary>
    public ChampionshipTeamsQuery GetChampionshipTeamsAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<ChampionshipTeam>("championship_teams", qs, c), ResolveTeamCarImagesAsync, ct);

    internal async Task ResolveTeamCarImagesAsync(ChampionshipTeam team, CancellationToken ct)
    {
        (team.CarLeftUrl, team.CarRightUrl) = await TeamCarImageResolver.ResolveCarUrlsAsync(_httpClient, team.TeamName, ct).ConfigureAwait(false);
    }
}
