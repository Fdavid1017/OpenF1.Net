using OpenF1.Net.Models.Enums;
using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class StintsTests
{
    [Fact]
    public async Task Deserializes_tyre_compound_and_nullable_lap_start()
    {
        var (api, _) = MockHttpFactory.ForFixture("stints", "Stints.json");

        var data = await api.GetStintsAsync();

        Assert.Equal(2, data.Length);
        var first = data[0];
        Assert.Equal(TyreCompound.Soft, first.Compound);
        Assert.Equal(1, first.DriverNumber);
        Assert.Equal(15, first.LapEnd);
        Assert.Equal(1, first.LapStart);
        Assert.Equal(1219, first.MeetingKey);
        Assert.Equal(9161, first.SessionKey);
        Assert.Equal(1, first.StintNumber);
        Assert.Equal(0, first.TyreAgeAtStart);

        var inProgress = data[1];
        Assert.Equal(TyreCompound.Medium, inProgress.Compound);
        Assert.Null(inProgress.LapEnd);
        Assert.Null(inProgress.LapStart);
    }
}
