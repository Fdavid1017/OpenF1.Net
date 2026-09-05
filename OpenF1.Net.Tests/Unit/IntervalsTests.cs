using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class IntervalsTests
{
    [Fact]
    public async Task Deserializes_leader_numeric_and_lapped_gap_shapes()
    {
        var (api, _) = MockHttpFactory.ForFixture("intervals", "Intervals.json");

        var data = await api.GetIntervalsAsync();

        Assert.Equal(3, data.Length);

        var leader = data[0];
        Assert.Null(leader.GapToLeader);
        Assert.Null(leader.IntervalValue);

        var numeric = data[1];
        Assert.NotNull(numeric.GapToLeader);
        Assert.Equal(5.123, numeric.GapToLeader!.Value.Seconds);
        Assert.False(numeric.GapToLeader.Value.IsLapped);
        Assert.NotNull(numeric.IntervalValue);
        Assert.Equal(1.456, numeric.IntervalValue!.Value.Seconds);

        var lapped = data[2];
        Assert.NotNull(lapped.GapToLeader);
        Assert.True(lapped.GapToLeader!.Value.IsLapped);
        Assert.Equal("+1 LAP", lapped.GapToLeader.Value.LapsBehind);
        Assert.Equal("+1 LAP", lapped.GapToLeader.Value.ToString());
    }

    [Fact]
    public async Task IncludeDriverDetails_attaches_each_row_its_own_driver()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("intervals", "Intervals.json");
        const string verstappenOnly = """[{"driver_number":1,"last_name":"Verstappen","session_key":9161,"meeting_key":1219}]""";
        const string perezOnly = """[{"driver_number":11,"last_name":"Perez","session_key":9161,"meeting_key":1219}]""";
        const string hamiltonOnly = """[{"driver_number":44,"last_name":"Hamilton","session_key":9161,"meeting_key":1219}]""";
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=1").Respond("application/json", verstappenOnly);
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=11").Respond("application/json", perezOnly);
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=44").Respond("application/json", hamiltonOnly);

        var data = await api.GetIntervalsAsync().IncludeDriverDetails();

        Assert.Equal("Verstappen", data[0].DriverDetails!.LastName);
        Assert.Equal("Perez", data[1].DriverDetails!.LastName);
        Assert.Equal("Hamilton", data[2].DriverDetails!.LastName);
    }
}
