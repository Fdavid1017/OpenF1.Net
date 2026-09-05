using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class SessionResultTests
{
    [Fact]
    public async Task Deserializes_scalar_and_qualifying_segment_shapes()
    {
        var (api, _) = MockHttpFactory.ForFixture("session_result", "SessionResult.json");

        var data = await api.GetSessionResultAsync();

        Assert.Equal(4, data.Length);

        var raceWinner = data[0];
        Assert.False(raceWinner.Dnf);
        Assert.Equal(1, raceWinner.Position);
        Assert.NotNull(raceWinner.Duration);
        Assert.Equal(5636.736, raceWinner.Duration!.Session);
        Assert.Null(raceWinner.Duration.Q1);
        Assert.Null(raceWinner.GapToLeader);

        var raceNumericGap = data[1];
        Assert.NotNull(raceNumericGap.GapToLeader);
        Assert.NotNull(raceNumericGap.GapToLeader!.Session);
        Assert.Equal(5.222, raceNumericGap.GapToLeader.Session!.Value.Seconds);
        Assert.False(raceNumericGap.GapToLeader.Session.Value.IsLapped);

        var lappedDnf = data[2];
        Assert.True(lappedDnf.Dnf);
        Assert.Null(lappedDnf.Position);
        Assert.Null(lappedDnf.Duration);
        Assert.NotNull(lappedDnf.GapToLeader);
        Assert.True(lappedDnf.GapToLeader!.Session!.Value.IsLapped);
        Assert.Equal("+1 LAP", lappedDnf.GapToLeader.Session.Value.LapsBehind);

        var qualifying = data[3];
        Assert.Null(qualifying.NumberOfLaps);
        Assert.NotNull(qualifying.Duration);
        Assert.Null(qualifying.Duration!.Session);
        Assert.Equal(88.235, qualifying.Duration.Q1);
        Assert.Equal(87.912, qualifying.Duration.Q2);
        Assert.Null(qualifying.Duration.Q3);
        Assert.NotNull(qualifying.GapToLeader);
        Assert.Null(qualifying.GapToLeader!.Session);
        Assert.Equal(0.145, qualifying.GapToLeader.Q1);
        Assert.Equal(0.221, qualifying.GapToLeader.Q2);
        Assert.Null(qualifying.GapToLeader.Q3);
    }

    [Fact]
    public async Task IncludeDriverDetails_keys_the_lookup_by_session_and_driver_number_together()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("session_result", "SessionResult.json");
        const string verstappenOnly = """[{"driver_number":1,"last_name":"Verstappen","session_key":9161,"meeting_key":1219}]""";
        const string perezOnly = """[{"driver_number":11,"last_name":"Perez","session_key":9161,"meeting_key":1219}]""";
        const string hamiltonOnly = """[{"driver_number":44,"last_name":"Hamilton","session_key":9161,"meeting_key":1219}]""";
        // driver_number=63 exists only under session_key=9160 in the fixture — a different session than the others.
        const string russellOnly = """[{"driver_number":63,"last_name":"Russell","session_key":9160,"meeting_key":1219}]""";
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=1").Respond("application/json", verstappenOnly);
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=11").Respond("application/json", perezOnly);
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=44").Respond("application/json", hamiltonOnly);
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9160&driver_number=63").Respond("application/json", russellOnly);

        var data = await api.GetSessionResultAsync().IncludeDriverDetails();

        Assert.Equal("Verstappen", data[0].DriverDetails!.LastName);
        Assert.Equal("Perez", data[1].DriverDetails!.LastName);
        Assert.Equal("Hamilton", data[2].DriverDetails!.LastName);
        Assert.Equal("Russell", data[3].DriverDetails!.LastName);
    }
}
