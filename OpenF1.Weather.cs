using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1
{
    /// <summary>The weather over the track, updated every minute.</summary>
    public WeatherQuery GetWeatherAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<Weather>("weather", qs, c), ct);
}
