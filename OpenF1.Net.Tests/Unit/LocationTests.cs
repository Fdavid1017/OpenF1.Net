using OpenF1.Net.Tests.TestHelpers;

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
}
