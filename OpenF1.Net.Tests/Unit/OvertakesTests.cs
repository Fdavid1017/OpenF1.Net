using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class OvertakesTests
{
    [Fact]
    public async Task Deserializes_all_fields()
    {
        var (api, _) = MockHttpFactory.ForFixture("overtakes", "Overtakes.json");

        var data = await api.GetOvertakesAsync();

        Assert.Single(data);
        var overtake = data[0];
        Assert.Equal(1219, overtake.MeetingKey);
        Assert.Equal(11, overtake.OvertakenDriverNumber);
        Assert.Equal(1, overtake.OvertakingDriverNumber);
        Assert.Equal(1, overtake.Position);
        Assert.Equal(9161, overtake.SessionKey);
    }
}
