using System.Net;
using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class ChampionshipTeamsTests
{
    [Fact]
    public async Task Deserializes_all_fields()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("championship_teams", "ChampionshipTeams.json");
        mockHttp.Fallback.Respond(HttpStatusCode.NotFound);

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

    [Fact]
    public async Task Always_resolves_car_urls_without_an_opt_in_call()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("championship_teams", "ChampionshipTeams.json");
        mockHttp.Fallback.Respond(HttpStatusCode.NotFound);

        // "Red Bull Racing" -> "redbullracing" team slug; only this year's left render exists.
        var year = DateTime.UtcNow.Year;
        mockHttp
            .When($"https://media.formula1.com/image/upload/c_lfill,w_3392/q_auto/common/f1/{year}/redbullracing/{year}redbullracingcarleft.webp")
            .Respond(HttpStatusCode.OK);

        var data = await api.GetChampionshipTeamsAsync();

        var redBull = Assert.Single(data, t => t.TeamName == "Red Bull Racing");
        Assert.Equal(
            $"https://media.formula1.com/image/upload/c_lfill,w_3392/q_auto/common/f1/{year}/redbullracing/{year}redbullracingcarleft.webp",
            redBull.CarLeftUrl
        );
        Assert.Null(redBull.CarRightUrl);

        var mercedes = Assert.Single(data, t => t.TeamName == "Mercedes");
        Assert.Null(mercedes.CarLeftUrl);
        Assert.Null(mercedes.CarRightUrl);
    }

    [Fact]
    public async Task Haas_F1_Team_uses_its_special_case_slug_instead_of_the_generic_one()
    {
        var mockHttp = new MockHttpMessageHandler();
        const string haasOnly = """[{"meeting_key":1219,"points_current":10.0,"points_start":8.0,"position_current":10,"position_start":10,"session_key":9161,"team_name":"Haas F1 Team"}]""";
        mockHttp.When("https://api.openf1.org/v1/championship_teams*").Respond("application/json", haasOnly);
        mockHttp.Fallback.Respond(HttpStatusCode.NotFound);

        // "Haas F1 Team" doesn't slug down to "haasf1team" the way every other team name does — it's a
        // hardcoded override to "haas", which is what F1's own asset paths actually use.
        var year = DateTime.UtcNow.Year;
        mockHttp
            .When($"https://media.formula1.com/image/upload/c_lfill,w_3392/q_auto/common/f1/{year}/haas/{year}haascarleft.webp")
            .Respond(HttpStatusCode.OK);

        var api = new OpenF1Client(mockHttp.ToHttpClient(), new OpenF1Config { UseRateLimit = false });
        var data = await api.GetChampionshipTeamsAsync();

        var haas = Assert.Single(data);
        Assert.Equal($"https://media.formula1.com/image/upload/c_lfill,w_3392/q_auto/common/f1/{year}/haas/{year}haascarleft.webp", haas.CarLeftUrl);
    }
}
