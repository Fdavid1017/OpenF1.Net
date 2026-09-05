using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class StartingGridTests
{
    [Fact]
    public async Task Deserializes_all_fields_including_nullable_lap_duration()
    {
        var (api, _) = MockHttpFactory.ForFixture("starting_grid", "StartingGrid.json");

        var data = await api.GetStartingGridAsync();

        Assert.Equal(2, data.Length);
        var first = data[0];
        Assert.Equal(1, first.DriverNumber);
        Assert.Equal(89.372, first.LapDuration);
        Assert.Equal(1219, first.MeetingKey);
        Assert.Equal(1, first.Position);
        Assert.Equal(9161, first.SessionKey);

        Assert.Null(data[1].LapDuration);
    }

    [Fact]
    public async Task IncludeDriverDetails_attaches_each_row_its_own_driver()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("starting_grid", "StartingGrid.json");
        const string verstappenOnly = """[{"driver_number":1,"last_name":"Verstappen","session_key":9161,"meeting_key":1219}]""";
        const string perezOnly = """[{"driver_number":11,"last_name":"Perez","session_key":9161,"meeting_key":1219}]""";
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=1").Respond("application/json", verstappenOnly);
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=11").Respond("application/json", perezOnly);

        var data = await api.GetStartingGridAsync().IncludeDriverDetails();

        Assert.Equal("Verstappen", data[0].DriverDetails!.LastName);
        Assert.Equal("Perez", data[1].DriverDetails!.LastName);
    }
}
