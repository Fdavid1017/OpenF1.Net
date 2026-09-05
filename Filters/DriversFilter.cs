using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class DriversFilterFields
{
    public string BroadcastName => throw new NotSupportedException();
    public int DriverNumber => throw new NotSupportedException();
    public string FirstName => throw new NotSupportedException();
    public string FullName => throw new NotSupportedException();
    public string LastName => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public string NameAcronym => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
    public string TeamName => throw new NotSupportedException();
}

public class DriversQuery(
    Func<string, CancellationToken, Task<Driver[]>> execute,
    Func<Driver, CancellationToken, Task> resolveImages,
    CancellationToken ct
) : EndpointQuery<DriversFilterFields, Driver>(execute, ct)
{
    bool _resolveImages;

    /// <summary>
    /// Also resolves each driver's <see cref="Driver.HeadshotUrl"/> to the highest-resolution official F1
    /// headshot available, and populates <see cref="Driver.FullBodyUrlLeft"/> / <see cref="Driver.FullBodyUrlRight"/>
    /// with full-body renders for the driver's current team — several extra HEAD requests per driver, sent to
    /// media.formula1.com and assets.multiviewer.dev rather than the OpenF1Client API.
    /// </summary>
    public DriversQuery ResolveImages()
    {
        _resolveImages = true;
        return this;
    }

    protected override async Task<Driver[]> ExecuteAsync()
    {
        var drivers = await base.ExecuteAsync().ConfigureAwait(false);
        if (!_resolveImages)
            return drivers;

        foreach (var driver in drivers)
            await resolveImages(driver, CancellationToken).ConfigureAwait(false);

        return drivers;
    }
}
