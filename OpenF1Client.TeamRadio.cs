using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1Client
{
    /// <summary>A collection of radio exchanges between F1 drivers and their teams during sessions. Only a limited selection is included.</summary>
    public TeamRadioQuery GetTeamRadioAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<TeamRadioMessage>("team_radio", qs, c), FetchDriverDetailsAsync, ct);
}
