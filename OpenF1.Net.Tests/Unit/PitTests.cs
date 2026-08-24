using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class PitTests
{
    [Fact]
    public async Task Deserializes_all_fields_including_nullable_stop_duration()
    {
        var (api, _) = MockHttpFactory.ForFixture("pit", "Pit.json");

        var data = await api.GetPitAsync();

        Assert.Equal(2, data.Length);
        var first = data[0];
        Assert.Equal(1, first.DriverNumber);
        Assert.Equal(22.456, first.LaneDuration);
        Assert.Equal(15, first.LapNumber);
        Assert.Equal(1219, first.MeetingKey);
#pragma warning disable CS0618 // deprecated field, still asserted while it mirrors LaneDuration
        Assert.Equal(22.456, first.PitDuration);
#pragma warning restore CS0618
        Assert.Equal(9161, first.SessionKey);
        Assert.Equal(2.4, first.StopDuration);

        Assert.Null(data[1].StopDuration);
    }
}
