using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class StartingGridTests
{
    [Fact]
    public async Task Deserializes_all_fields_including_nullable_lap_duration()
    {
        var (api, _) = MockHttpFactory.ForFixture("starting_grid", "StartingGrid.json");

        var data = await api.GetStartingGridAsync();

        Assert.Equal(2, data.Length);
        var first = data[0];
        Assert.Equal(1, first.DriverNumber);
        Assert.Equal(89.372, first.LapDuration);
        Assert.Equal(1219, first.MeetingKey);
        Assert.Equal(1, first.Position);
        Assert.Equal(9161, first.SessionKey);

        Assert.Null(data[1].LapDuration);
    }
}
