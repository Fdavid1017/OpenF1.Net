using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class LocationTests
{
    [Fact]
    public async Task Deserializes_all_fields()
    {
        var (api, _) = MockHttpFactory.ForFixture("location", "Location.json");

        var data = await api.GetLocationAsync();

        Assert.Single(data);
        var point = data[0];
        Assert.Equal(1, point.DriverNumber);
        Assert.Equal(1219, point.MeetingKey);
        Assert.Equal(9161, point.SessionKey);
        Assert.Equal(-5107, point.X);
        Assert.Equal(2385, point.Y);
        Assert.Equal(190, point.Z);
    }

    [Fact]
    public async Task IncludeDriverDetails_attaches_the_matching_driver()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("location", "Location.json");
        const string verstappenOnly = """[{"driver_number":1,"last_name":"Verstappen","session_key":9161,"meeting_key":1219}]""";
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=1").Respond("application/json", verstappenOnly);

        var data = await api.GetLocationAsync().IncludeDriverDetails();

        var point = Assert.Single(data);
        Assert.NotNull(point.DriverDetails);
        Assert.Equal(1, point.DriverDetails!.DriverNumber);
        Assert.Equal("Verstappen", point.DriverDetails.LastName);
    }
}
