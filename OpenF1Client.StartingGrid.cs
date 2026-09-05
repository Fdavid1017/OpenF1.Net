using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1Client
{
    /// <summary>The starting grid for the upcoming race. Becomes available a few minutes after official results are published.</summary>
    public StartingGridQuery GetStartingGridAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<StartingGridPosition>("starting_grid", qs, c), FetchDriverDetailsAsync, ct);
}
