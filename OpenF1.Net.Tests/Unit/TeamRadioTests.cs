using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class TeamRadioTests
{
    [Fact]
    public async Task Deserializes_all_fields()
    {
        var (api, _) = MockHttpFactory.ForFixture("team_radio", "TeamRadio.json");

        var data = await api.GetTeamRadioAsync();

        Assert.Single(data);
        var message = data[0];
        Assert.Equal(1, message.DriverNumber);
        Assert.Equal(1219, message.MeetingKey);
        Assert.Equal("https://example.com/radio/1.mp3", message.RecordingUrl);
        Assert.Equal(9161, message.SessionKey);
    }

    [Fact]
    public async Task IncludeDriverDetails_attaches_the_matching_driver()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("team_radio", "TeamRadio.json");
        const string verstappenOnly = """[{"driver_number":1,"last_name":"Verstappen","session_key":9161,"meeting_key":1219}]""";
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=1").Respond("application/json", verstappenOnly);

        var data = await api.GetTeamRadioAsync().IncludeDriverDetails();

        var message = Assert.Single(data);
        Assert.NotNull(message.DriverDetails);
        Assert.Equal(1, message.DriverDetails!.DriverNumber);
        Assert.Equal("Verstappen", message.DriverDetails.LastName);
    }
}
