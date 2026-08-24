using OpenF1.Net.Models.Enums;
using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class MeetingsTests
{
    [Fact]
    public async Task Deserializes_all_three_circuit_type_values()
    {
        var (api, _) = MockHttpFactory.ForFixture("meetings", "Meetings.json");

        var data = await api.GetMeetingsAsync();

        Assert.Equal(3, data.Length);
        var singapore = data[0];
        Assert.Equal(61, singapore.CircuitKey);
        Assert.Equal("https://example.com/circuit/61.json", singapore.CircuitInfoUrl);
        Assert.Equal("Marina Bay", singapore.CircuitShortName);
        Assert.Equal(CircuitType.TemporaryStreet, singapore.CircuitType);
        Assert.Equal("SGP", singapore.CountryCode);
        Assert.Equal("https://example.com/flags/sgp.png", singapore.CountryFlag);
        Assert.Equal(157, singapore.CountryKey);
        Assert.Equal("Singapore", singapore.CountryName);
        Assert.Equal("08:00:00", singapore.GmtOffset);
        Assert.False(singapore.IsCancelled);
        Assert.Equal("Marina Bay", singapore.Location);
        Assert.Equal(1219, singapore.MeetingKey);
        Assert.Equal("Singapore Grand Prix", singapore.MeetingName);
        Assert.Equal("FORMULA 1 SINGAPORE AIRLINES SINGAPORE GRAND PRIX 2023", singapore.MeetingOfficialName);
        Assert.Equal(2023, singapore.Year);

        Assert.Equal(CircuitType.Permanent, data[1].CircuitType);
        Assert.Equal(CircuitType.TemporaryStreetRoad, data[2].CircuitType);
        Assert.True(data[2].IsCancelled);
    }
}
