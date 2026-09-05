using OpenF1.Net.Filters;
using OpenF1.Net.Internal;
using OpenF1.Net.Models;

namespace OpenF1.Net;

public partial class OpenF1Client
{
    /// <summary>Detailed information about the drivers participating in a specific session.</summary>
    public DriversQuery GetDriversAsync(CancellationToken ct = default) =>
        new((qs, c) => ExecuteAsync<Driver>("drivers", qs, c), ResolveDriverImagesAsync, ct);

    /// <summary>
    /// Resolves a driver's headshot to the highest-resolution official F1 asset available (falling back to
    /// MultiViewer's mirror only when F1's own asset is missing), and looks up full-body left/right renders
    /// for their current team. No image is downloaded — only HEAD requests confirm each candidate URL exists.
    /// Populated only when the query is built with .ResolveImages().
    /// </summary>
    internal async Task ResolveDriverImagesAsync(Driver driver, CancellationToken ct)
    {
        var headshotUrl = await DriverImageResolver
            .ResolveHeadshotUrlAsync(_httpClient, driver.FirstName, driver.LastName, driver.NameAcronym, ct)
            .ConfigureAwait(false);
        if (headshotUrl is not null)
            driver.HeadshotUrl = headshotUrl;

        (driver.FullBodyUrlLeft, driver.FullBodyUrlRight) = await DriverImageResolver
            .ResolveFullBodyUrlsAsync(_httpClient, driver.FirstName, driver.LastName, driver.TeamName, ct)
            .ConfigureAwait(false);
    }
}
