using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1
{
    /// <summary>Championship standings for drivers. Only available for race sessions.</summary>
    public ChampionshipDriversQuery GetChampionshipDriversAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<ChampionshipDriver>("championship_drivers", qs, c), ct);
}
