using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class LatestConvenienceTests
{
    [Fact]
    public async Task GetLatestSessionAsync_returns_the_first_element_when_present()
    {
        var (api, _) = MockHttpFactory.ForFixture("sessions", "Sessions.json");

        var session = await api.GetLatestSessionAsync();

        Assert.NotNull(session);
        Assert.Equal(9158, session!.SessionKey);
    }

    [Fact]
    public async Task GetLatestSessionAsync_returns_null_when_the_api_returns_no_results()
    {
        var (api, _) = MockHttpFactory.ForFixture("sessions", "Empty.json");

        var session = await api.GetLatestSessionAsync();

        Assert.Null(session);
    }

    [Fact]
    public async Task GetLatestMeetingAsync_returns_the_first_element_when_present()
    {
        var (api, _) = MockHttpFactory.ForFixture("meetings", "Meetings.json");

        var meeting = await api.GetLatestMeetingAsync();

        Assert.NotNull(meeting);
        Assert.Equal(1219, meeting!.MeetingKey);
    }

    [Fact]
    public async Task GetLatestMeetingAsync_returns_null_when_the_api_returns_no_results()
    {
        var (api, _) = MockHttpFactory.ForFixture("meetings", "Empty.json");

        var meeting = await api.GetLatestMeetingAsync();

        Assert.Null(meeting);
    }
}
