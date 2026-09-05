using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class PositionTests
{
    [Fact]
    public async Task Deserializes_position_field_despite_json_property_rename()
    {
        var (api, _) = MockHttpFactory.ForFixture("position", "Position.json");

        var data = await api.GetPositionAsync();

        Assert.Equal(2, data.Length);
        var first = data[0];
        Assert.Equal(1, first.DriverNumber);
        Assert.Equal(1219, first.MeetingKey);
        Assert.Equal(1, first.PositionValue);
        Assert.Equal(9161, first.SessionKey);
        Assert.Equal(2, data[1].PositionValue);
    }

    [Fact]
    public async Task IncludeDriverDetails_attaches_each_row_its_own_driver()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("position", "Position.json");
        const string verstappenOnly = """[{"driver_number":1,"last_name":"Verstappen","session_key":9161,"meeting_key":1219}]""";
        const string perezOnly = """[{"driver_number":11,"last_name":"Perez","session_key":9161,"meeting_key":1219}]""";
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=1").Respond("application/json", verstappenOnly);
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=11").Respond("application/json", perezOnly);

        var data = await api.GetPositionAsync().IncludeDriverDetails();

        Assert.Equal("Verstappen", data[0].DriverDetails!.LastName);
        Assert.Equal("Perez", data[1].DriverDetails!.LastName);
    }
}
