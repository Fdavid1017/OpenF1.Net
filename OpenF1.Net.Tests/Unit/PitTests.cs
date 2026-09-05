using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class PitTests
{
    [Fact]
    public async Task Deserializes_all_fields_including_nullable_stop_duration()
    {
        var (api, _) = MockHttpFactory.ForFixture("pit", "Pit.json");

        var data = await api.GetPitAsync();

        Assert.Equal(2, data.Length);
        var first = data[0];
        Assert.Equal(1, first.DriverNumber);
        Assert.Equal(22.456, first.LaneDuration);
        Assert.Equal(15, first.LapNumber);
        Assert.Equal(1219, first.MeetingKey);
#pragma warning disable CS0618 // deprecated field, still asserted while it mirrors LaneDuration
        Assert.Equal(22.456, first.PitDuration);
#pragma warning restore CS0618
        Assert.Equal(9161, first.SessionKey);
        Assert.Equal(2.4, first.StopDuration);

        Assert.Null(data[1].StopDuration);
    }

    [Fact]
    public async Task IncludeDriverDetails_attaches_each_row_its_own_driver()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("pit", "Pit.json");
        const string verstappenOnly = """[{"driver_number":1,"last_name":"Verstappen","session_key":9161,"meeting_key":1219}]""";
        const string perezOnly = """[{"driver_number":11,"last_name":"Perez","session_key":9161,"meeting_key":1219}]""";
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=1").Respond("application/json", verstappenOnly);
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=11").Respond("application/json", perezOnly);

        var data = await api.GetPitAsync().IncludeDriverDetails();

        Assert.Equal("Verstappen", data[0].DriverDetails!.LastName);
        Assert.Equal("Perez", data[1].DriverDetails!.LastName);
    }
}
