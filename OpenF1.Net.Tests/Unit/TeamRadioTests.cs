using OpenF1.Net.Tests.TestHelpers;

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
}
