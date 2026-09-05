using OpenF1.Net.Filters;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1Client
{
    /// <summary>Some data about each car, at a sample rate of about 3.7 Hz.</summary>
    public CarDataQuery GetCarDataAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<CarDataPoint>("car_data", qs, c), FetchDriverDetailsAsync, ct);
}
