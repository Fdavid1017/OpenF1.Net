using OpenF1.Net.Models.Enums;
using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class LapsTests
{
    [Fact]
    public async Task Deserializes_segment_status_and_nullable_fields()
    {
        var (api, _) = MockHttpFactory.ForFixture("laps", "Laps.json");

        var data = await api.GetLapsAsync();

        Assert.Equal(2, data.Length);
        var first = data[0];
        Assert.Equal(1, first.DriverNumber);
        Assert.Equal(28.456, first.DurationSector1);
        Assert.Equal(30.112, first.DurationSector2);
        Assert.Equal(26.789, first.DurationSector3);
        Assert.Equal(285, first.I1Speed);
        Assert.Equal(301, first.I2Speed);
        Assert.False(first.IsPitOutLap);
        Assert.Equal(85.357, first.LapDuration);
        Assert.Equal(8, first.LapNumber);
        Assert.Equal(312, first.StSpeed);
        Assert.Equal([SegmentStatus.Green, SegmentStatus.Green, SegmentStatus.Purple], first.SegmentsSector1);
        Assert.Equal([SegmentStatus.Unavailable, SegmentStatus.Pitlane, SegmentStatus.Unknown], first.SegmentsSector2);
        Assert.Equal([SegmentStatus.Yellow, SegmentStatus.Green, SegmentStatus.Green], first.SegmentsSector3);

        var inProgress = data[1];
        Assert.True(inProgress.IsPitOutLap);
        Assert.Null(inProgress.LapDuration);
        Assert.Null(inProgress.DurationSector1);
        Assert.Null(inProgress.StSpeed);
        Assert.Empty(inProgress.SegmentsSector1);
    }
}
