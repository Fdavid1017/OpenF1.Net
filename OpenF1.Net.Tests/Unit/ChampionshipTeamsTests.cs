using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class ChampionshipTeamsTests
{
    [Fact]
    public async Task Deserializes_all_fields()
    {
        var (api, _) = MockHttpFactory.ForFixture("championship_teams", "ChampionshipTeams.json");

        var data = await api.GetChampionshipTeamsAsync();

        Assert.Equal(2, data.Length);
        var first = data[0];
        Assert.Equal(1219, first.MeetingKey);
        Assert.Equal(636.0, first.PointsCurrent);
        Assert.Equal(599.0, first.PointsStart);
        Assert.Equal(1, first.PositionCurrent);
        Assert.Equal(1, first.PositionStart);
        Assert.Equal(9161, first.SessionKey);
        Assert.Equal("Red Bull Racing", first.TeamName);
    }
}
