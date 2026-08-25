using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1Client
{
    /// <summary>Information about cars going through the pit lane.</summary>
    public PitQuery GetPitAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<PitStop>("pit", qs, c), ct);
}
