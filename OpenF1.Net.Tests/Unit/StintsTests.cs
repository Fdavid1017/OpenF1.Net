using OpenF1.Net.Models.Enums;
using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

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

    [Fact]
    public async Task IncludeDriverDetails_attaches_the_matching_driver_and_dedups_by_session_and_driver_number()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("stints", "Stints.json");
        const string verstappenOnly = """[{"driver_number":1,"last_name":"Verstappen","session_key":9161,"meeting_key":1219}]""";
        // Both fixture stints share driver_number=1/session_key=9161 — only one /drivers call should happen for the two of them.
        var driversRequest = mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=1").Respond("application/json", verstappenOnly);

        var data = await api.GetStintsAsync().IncludeDriverDetails();

        Assert.Equal(2, data.Length);
        Assert.All(
            data,
            s =>
            {
                Assert.NotNull(s.DriverDetails);
                Assert.Equal(1, s.DriverDetails!.DriverNumber);
                Assert.Equal("Verstappen", s.DriverDetails.LastName);
            }
        );
        Assert.Equal(1, mockHttp.GetMatchCount(driversRequest));
    }
}
