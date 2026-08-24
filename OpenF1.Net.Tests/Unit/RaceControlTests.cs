using OpenF1.Net.Models.Enums;
using OpenF1.Net.Tests.TestHelpers;

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
}
