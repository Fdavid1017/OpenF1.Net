using OpenF1.Net.Models.Enums;
using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class SessionsTests
{
    [Fact]
    public async Task Deserializes_all_fields()
    {
        var (api, _) = MockHttpFactory.ForFixture("sessions", "Sessions.json");

        var data = await api.GetSessionsAsync();

        Assert.Equal(5, data.Length);
        var practice = data[0];
        Assert.Equal(61, practice.CircuitKey);
        Assert.Equal("Marina Bay", practice.CircuitShortName);
        Assert.Equal("SGP", practice.CountryCode);
        Assert.Equal(157, practice.CountryKey);
        Assert.Equal("Singapore", practice.CountryName);
        Assert.Equal("08:00:00", practice.GmtOffset);
        Assert.False(practice.IsCancelled);
        Assert.Equal("Marina Bay", practice.Location);
        Assert.Equal(1219, practice.MeetingKey);
        Assert.Equal(9158, practice.SessionKey);
        Assert.Equal(SessionName.Practice1, practice.SessionName);
        Assert.Equal(SessionType.Practice, practice.SessionType);
        Assert.Equal(2023, practice.Year);
    }

    [Fact]
    public async Task Maps_sprint_session_names_to_race_and_qualifying_types()
    {
        var (api, _) = MockHttpFactory.ForFixture("sessions", "Sessions.json");

        var data = await api.GetSessionsAsync();

        var sprintQualifying = data[3];
        Assert.Equal(SessionName.SprintQualifying, sprintQualifying.SessionName);
        Assert.Equal(SessionType.Qualifying, sprintQualifying.SessionType);

        var sprint = data[4];
        Assert.Equal(SessionName.Sprint, sprint.SessionName);
        Assert.Equal(SessionType.Race, sprint.SessionType);
    }
}
