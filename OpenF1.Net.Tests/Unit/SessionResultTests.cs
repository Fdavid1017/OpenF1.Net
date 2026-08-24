using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class SessionResultTests
{
    [Fact]
    public async Task Deserializes_scalar_and_qualifying_segment_shapes()
    {
        var (api, _) = MockHttpFactory.ForFixture("session_result", "SessionResult.json");

        var data = await api.GetSessionResultAsync();

        Assert.Equal(4, data.Length);

        var raceWinner = data[0];
        Assert.False(raceWinner.Dnf);
        Assert.Equal(1, raceWinner.Position);
        Assert.NotNull(raceWinner.Duration);
        Assert.Equal(5636.736, raceWinner.Duration!.Session);
        Assert.Null(raceWinner.Duration.Q1);
        Assert.Null(raceWinner.GapToLeader);

        var raceNumericGap = data[1];
        Assert.NotNull(raceNumericGap.GapToLeader);
        Assert.NotNull(raceNumericGap.GapToLeader!.Session);
        Assert.Equal(5.222, raceNumericGap.GapToLeader.Session!.Value.Seconds);
        Assert.False(raceNumericGap.GapToLeader.Session.Value.IsLapped);

        var lappedDnf = data[2];
        Assert.True(lappedDnf.Dnf);
        Assert.Null(lappedDnf.Position);
        Assert.Null(lappedDnf.Duration);
        Assert.NotNull(lappedDnf.GapToLeader);
        Assert.True(lappedDnf.GapToLeader!.Session!.Value.IsLapped);
        Assert.Equal("+1 LAP", lappedDnf.GapToLeader.Session.Value.LapsBehind);

        var qualifying = data[3];
        Assert.Null(qualifying.NumberOfLaps);
        Assert.NotNull(qualifying.Duration);
        Assert.Null(qualifying.Duration!.Session);
        Assert.Equal(88.235, qualifying.Duration.Q1);
        Assert.Equal(87.912, qualifying.Duration.Q2);
        Assert.Null(qualifying.Duration.Q3);
        Assert.NotNull(qualifying.GapToLeader);
        Assert.Null(qualifying.GapToLeader!.Session);
        Assert.Equal(0.145, qualifying.GapToLeader.Q1);
        Assert.Equal(0.221, qualifying.GapToLeader.Q2);
        Assert.Null(qualifying.GapToLeader.Q3);
    }
}
