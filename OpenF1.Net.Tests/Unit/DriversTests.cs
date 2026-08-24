using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class DriversTests
{
    [Fact]
    public async Task Deserializes_all_fields()
    {
        var (api, _) = MockHttpFactory.ForFixture("drivers", "Drivers.json");

        var data = await api.GetDriversAsync();

        Assert.Equal(2, data.Length);
        var first = data[0];
        Assert.Equal("M VERSTAPPEN", first.BroadcastName);
#pragma warning disable CS0618 // deprecated field, still asserted while it's live
        Assert.Equal("NED", first.CountryCode);
#pragma warning restore CS0618
        Assert.Equal(1, first.DriverNumber);
        Assert.Equal("Max", first.FirstName);
        Assert.Equal("Max VERSTAPPEN", first.FullName);
        Assert.Equal("https://example.com/verstappen.png", first.HeadshotUrl);
        Assert.Equal("Verstappen", first.LastName);
        Assert.Equal(1219, first.MeetingKey);
        Assert.Equal("VER", first.NameAcronym);
        Assert.Equal(9161, first.SessionKey);
        Assert.Equal("3671C6", first.TeamColour);
        Assert.Equal("Red Bull Racing", first.TeamName);
    }
}
