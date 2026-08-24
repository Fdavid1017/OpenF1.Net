using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class IntervalsTests
{
    [Fact]
    public async Task Deserializes_leader_numeric_and_lapped_gap_shapes()
    {
        var (api, _) = MockHttpFactory.ForFixture("intervals", "Intervals.json");

        var data = await api.GetIntervalsAsync();

        Assert.Equal(3, data.Length);

        var leader = data[0];
        Assert.Null(leader.GapToLeader);
        Assert.Null(leader.IntervalValue);

        var numeric = data[1];
        Assert.NotNull(numeric.GapToLeader);
        Assert.Equal(5.123, numeric.GapToLeader!.Value.Seconds);
        Assert.False(numeric.GapToLeader.Value.IsLapped);
        Assert.NotNull(numeric.IntervalValue);
        Assert.Equal(1.456, numeric.IntervalValue!.Value.Seconds);

        var lapped = data[2];
        Assert.NotNull(lapped.GapToLeader);
        Assert.True(lapped.GapToLeader!.Value.IsLapped);
        Assert.Equal("+1 LAP", lapped.GapToLeader.Value.LapsBehind);
        Assert.Equal("+1 LAP", lapped.GapToLeader.Value.ToString());
    }
}
