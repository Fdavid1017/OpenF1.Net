namespace OpenF1.Net.Tests.Live;

/// <summary>
/// One shared <see cref="OpenF1"/> instance for the whole class, so its built-in rate limiter actually
/// paces every test method's real HTTP call to the API's 3 requests/second cap — separate instances
/// (and therefore separate limiters) racing in parallel would each think they're the only caller and
/// trip a real 429.
/// </summary>
public class LiveApiFixture : IAsyncLifetime
{
    public OpenF1 Api { get; private set; } = null!;

    public Task InitializeAsync()
    {
        Api = new OpenF1();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Api.DisposeAsync().AsTask();
}

/// <summary>
/// Opt-in, excluded from the default run (dotnet test --filter "Category!=Live"). Hits the real
/// https://api.openf1.org/v1 for all 18 endpoints with shallow assertions only (2xx + successful
/// deserialization) — this is what catches API-drift bugs that mocked fixtures, written from the same
/// assumptions as the code, structurally can't. See .scratch/openf1-net-wrapper/issues/13-testing-strategy.md.
/// </summary>
[Trait("Category", "Live")]
public class LiveEndpointTests(LiveApiFixture fixture) : IClassFixture<LiveApiFixture>
{
    // A completed, historical session (2023 Singapore GP race) — bounded, stable data instead of
    // querying the live/current session, which could be enormous or in progress.
    const int SessionKey = 9161;
    const int MeetingKey = 1219;
    const int DriverNumber = 1;

    OpenF1 _api => fixture.Api;

    [Fact]
    public async Task CarData_deserializes()
    {
        var data = await _api.GetCarDataAsync().Where(x => x.SessionKey == SessionKey).And(x => x.DriverNumber == DriverNumber);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task ChampionshipDrivers_deserializes()
    {
        var data = await _api.GetChampionshipDriversAsync().Where(x => x.SessionKey == SessionKey);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task ChampionshipTeams_deserializes()
    {
        var data = await _api.GetChampionshipTeamsAsync().Where(x => x.SessionKey == SessionKey);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Drivers_deserializes()
    {
        var data = await _api.GetDriversAsync().Where(x => x.SessionKey == SessionKey);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Intervals_deserializes()
    {
        var data = await _api.GetIntervalsAsync().Where(x => x.SessionKey == SessionKey);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Laps_deserializes()
    {
        var data = await _api.GetLapsAsync().Where(x => x.SessionKey == SessionKey).And(x => x.DriverNumber == DriverNumber);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Location_deserializes()
    {
        var data = await _api.GetLocationAsync().Where(x => x.SessionKey == SessionKey).And(x => x.DriverNumber == DriverNumber);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Meetings_deserializes()
    {
        var data = await _api.GetMeetingsAsync().Where(x => x.MeetingKey == MeetingKey);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Overtakes_deserializes()
    {
        var data = await _api.GetOvertakesAsync().Where(x => x.SessionKey == SessionKey);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Pit_deserializes()
    {
        var data = await _api.GetPitAsync().Where(x => x.SessionKey == SessionKey);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Position_deserializes()
    {
        var data = await _api.GetPositionAsync().Where(x => x.SessionKey == SessionKey).And(x => x.DriverNumber == DriverNumber);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task RaceControl_deserializes()
    {
        var data = await _api.GetRaceControlAsync().Where(x => x.SessionKey == SessionKey);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task SessionResult_deserializes()
    {
        var data = await _api.GetSessionResultAsync().Where(x => x.SessionKey == SessionKey);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Sessions_deserializes()
    {
        var data = await _api.GetSessionsAsync().Where(x => x.MeetingKey == MeetingKey);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task StartingGrid_deserializes()
    {
        var data = await _api.GetStartingGridAsync().Where(x => x.SessionKey == SessionKey);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Stints_deserializes()
    {
        var data = await _api.GetStintsAsync().Where(x => x.SessionKey == SessionKey).And(x => x.DriverNumber == DriverNumber);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task TeamRadio_deserializes()
    {
        var data = await _api.GetTeamRadioAsync().Where(x => x.SessionKey == SessionKey);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Weather_deserializes()
    {
        var data = await _api.GetWeatherAsync().Where(x => x.SessionKey == SessionKey);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task GetLatestSessionAsync_deserializes()
    {
        var session = await _api.GetLatestSessionAsync();
        Assert.True(session is null || session.SessionKey > 0);
    }

    [Fact]
    public async Task GetLatestMeetingAsync_deserializes()
    {
        var meeting = await _api.GetLatestMeetingAsync();
        Assert.True(meeting is null || meeting.MeetingKey > 0);
    }
}
