using OpenF1.Net.Tests.TestHelpers;

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
}
