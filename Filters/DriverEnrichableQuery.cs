using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

/// <summary>
/// Base for queries whose result model carries a driver number. Adds .IncludeDriverDetails(), which fetches
/// each result's matching /drivers record (looked up by session_key + driver_number) and attaches it as
/// DriverDetails — one request per distinct (session, driver) pair found in the results, reused across every
/// row sharing that pair.
/// </summary>
public abstract class DriverEnrichableQuery<TFields, TModel> : EndpointQuery<TFields, TModel>
{
    readonly Func<int, int, bool, CancellationToken, Task<Driver?>> _fetchDriverDetails;
    readonly Func<TModel, int> _getSessionKey;
    readonly Func<TModel, int?> _getDriverNumber;
    readonly Action<TModel, Driver?> _setDriverDetails;
    bool _includeDriverDetails;
    bool _resolveImages;

    protected DriverEnrichableQuery(
        Func<string, CancellationToken, Task<TModel[]>> execute,
        Func<int, int, bool, CancellationToken, Task<Driver?>> fetchDriverDetails,
        Func<TModel, int> getSessionKey,
        Func<TModel, int?> getDriverNumber,
        Action<TModel, Driver?> setDriverDetails,
        CancellationToken ct
    ) : base(execute, ct)
    {
        _fetchDriverDetails = fetchDriverDetails;
        _getSessionKey = getSessionKey;
        _getDriverNumber = getDriverNumber;
        _setDriverDetails = setDriverDetails;
    }

    /// <summary>
    /// Also fetches each result's driver from /drivers and attaches it as DriverDetails. Rows with no driver
    /// number (e.g. a RaceControlMessage not tied to a driver) are left with a null DriverDetails, as is any
    /// row whose (session, driver) pair has no matching driver — that's not treated as an error. Pass
    /// resolveImages: true to also resolve that driver's headshot/full-body render URLs, as
    /// DriversQuery.ResolveImages() would.
    /// </summary>
    public DriverEnrichableQuery<TFields, TModel> IncludeDriverDetails(bool resolveImages = false)
    {
        _includeDriverDetails = true;
        _resolveImages = resolveImages;
        return this;
    }

    protected override async Task<TModel[]> ExecuteAsync()
    {
        var results = await base.ExecuteAsync().ConfigureAwait(false);
        if (!_includeDriverDetails)
            return results;

        var cache = new Dictionary<(int SessionKey, int DriverNumber), Driver?>();
        foreach (var item in results)
        {
            var driverNumber = _getDriverNumber(item);
            if (driverNumber is null)
                continue;

            var key = (SessionKey: _getSessionKey(item), DriverNumber: driverNumber.Value);
            if (!cache.TryGetValue(key, out var driver))
            {
                driver = await _fetchDriverDetails(key.SessionKey, key.DriverNumber, _resolveImages, CancellationToken).ConfigureAwait(false);
                cache[key] = driver;
            }

            _setDriverDetails(item, driver);
        }

        return results;
    }
}
