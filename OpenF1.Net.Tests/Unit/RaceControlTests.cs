using OpenF1.Net.Models.Enums;
using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class RaceControlTests
{
    [Fact]
    public async Task Deserializes_category_flag_and_scope_including_fallbacks()
    {
        var (api, _) = MockHttpFactory.ForFixture("race_control", "RaceControl.json");

        var data = await api.GetRaceControlAsync();

        Assert.Equal(3, data.Length);

        var flagRow = data[0];
        Assert.Equal(Category.Flag, flagRow.Category);
        Assert.Equal(Flag.Yellow, flagRow.Flag);
        Assert.Equal(Scope.Sector, flagRow.Scope);
        Assert.Equal(5, flagRow.Sector);
        Assert.Null(flagRow.DriverNumber);
        Assert.Null(flagRow.QualifyingPhase);

        var statusRow = data[1];
        Assert.Equal(Category.SessionStatus, statusRow.Category);
        Assert.Null(statusRow.Flag);
        Assert.Null(statusRow.Scope);
        Assert.Equal("Session Started", statusRow.Message);

        var unknownCategoryRow = data[2];
        Assert.Equal(Category.Other, unknownCategoryRow.Category);
        Assert.Equal(Flag.BlackAndWhite, unknownCategoryRow.Flag);
        Assert.Equal(Scope.Driver, unknownCategoryRow.Scope);
        Assert.Equal(1, unknownCategoryRow.DriverNumber);
        Assert.Equal(2, unknownCategoryRow.QualifyingPhase);
    }

    [Fact]
    public async Task IncludeDriverDetails_skips_rows_with_no_driver_number()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("race_control", "RaceControl.json");
        const string verstappenOnly = """[{"driver_number":1,"last_name":"Verstappen","session_key":9161,"meeting_key":1219}]""";
        var driversRequest = mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=1").Respond("application/json", verstappenOnly);

        var data = await api.GetRaceControlAsync().IncludeDriverDetails();

        Assert.Null(data[0].DriverDetails);
        Assert.Null(data[1].DriverDetails);
        Assert.NotNull(data[2].DriverDetails);
        Assert.Equal("Verstappen", data[2].DriverDetails!.LastName);
        // Only the one row with a driver number should have triggered a /drivers call.
        Assert.Equal(1, mockHttp.GetMatchCount(driversRequest));
    }
}
