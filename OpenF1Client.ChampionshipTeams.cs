using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1Client
{
    /// <summary>Championship standings for teams. Only available for race sessions.</summary>
    public ChampionshipTeamsQuery GetChampionshipTeamsAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<ChampionshipTeam>("championship_teams", qs, c), ct);
}
