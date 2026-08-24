using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class ChampionshipDriversTests
{
    [Fact]
    public async Task Deserializes_all_fields()
    {
        var (api, _) = MockHttpFactory.ForFixture("championship_drivers", "ChampionshipDrivers.json");

        var data = await api.GetChampionshipDriversAsync();

        Assert.Equal(2, data.Length);
        var first = data[0];
        Assert.Equal(1, first.DriverNumber);
        Assert.Equal(1219, first.MeetingKey);
        Assert.Equal(429.0, first.PointsCurrent);
        Assert.Equal(404.0, first.PointsStart);
        Assert.Equal(1, first.PositionCurrent);
        Assert.Equal(1, first.PositionStart);
        Assert.Equal(9161, first.SessionKey);
    }
}
