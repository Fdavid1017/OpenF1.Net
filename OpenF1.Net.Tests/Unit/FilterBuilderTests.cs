using OpenF1.Net.Filters;
using OpenF1.Net.Models.Enums;

namespace OpenF1.Net.Tests.Unit;

public class FilterBuilderTests
{
    [Fact]
    public void Where_and_And_chain_into_ampersand_separated_clauses()
    {
        var builder = new FilterBuilder<DriversFilterFields>()
            .Where(x => x.DriverNumber == 1)
            .And(x => x.TeamName == "Red Bull Racing");

        Assert.Equal("driver_number=1&team_name=Red Bull Racing", builder.ToQueryString());
    }

    [Theory]
    [InlineData(">", 120)]
    [InlineData(">=", 120)]
    [InlineData("<", 120)]
    [InlineData("<=", 120)]
    public void Comparison_operators_render_as_the_api_expects(string op, int value)
    {
        FilterBuilder<LapsFilterFields> builder = op switch
        {
            ">" => new FilterBuilder<LapsFilterFields>().Where(x => x.LapNumber > value),
            ">=" => new FilterBuilder<LapsFilterFields>().Where(x => x.LapNumber >= value),
            "<" => new FilterBuilder<LapsFilterFields>().Where(x => x.LapNumber < value),
            "<=" => new FilterBuilder<LapsFilterFields>().Where(x => x.LapNumber <= value),
            _ => throw new InvalidOperationException(),
        };

        Assert.Equal($"lap_number{op}120", builder.ToQueryString());
    }

    [Fact]
    public void Enum_equality_renders_the_ApiValue_string_not_the_member_name()
    {
        var builder = new FilterBuilder<RaceControlFilterFields>().Where(x => x.Flag == Flag.BlackAndWhite);

        Assert.Equal("flag=BLACK AND WHITE", builder.ToQueryString());
    }

    [Fact]
    public void DateTime_renders_as_UTC_ISO8601_with_Z_suffix()
    {
        var builder = new FilterBuilder<LapsFilterFields>()
            .Where(x => x.DateStart >= new DateTime(2023, 9, 15, 12, 0, 0, DateTimeKind.Utc));

        Assert.Equal("date_start>=2023-09-15T12:00:00Z", builder.ToQueryString());
    }

    [Fact]
    public void Bool_renders_as_lowercase_true_or_false()
    {
        var builder = new FilterBuilder<LapsFilterFields>().Where(x => x.IsPitOutLap == true);

        Assert.Equal("is_pit_out_lap=true", builder.ToQueryString());
    }

    [Fact]
    public void SessionKeyRef_and_MeetingKeyRef_render_latest_or_the_numeric_key()
    {
        var latest = new FilterBuilder<DriversFilterFields>().Where(x => x.SessionKey == SessionKeyRef.Latest);
        var numeric = new FilterBuilder<DriversFilterFields>().Where(x => x.MeetingKey == 1219);

        Assert.Equal("session_key=latest", latest.ToQueryString());
        Assert.Equal("meeting_key=1219", numeric.ToQueryString());
    }

    [Fact]
    public void WhereIn_repeats_the_same_key_for_each_value()
    {
        var builder = new FilterBuilder<DriversFilterFields>().WhereIn(x => x.DriverNumber, 1, 11, 44);

        Assert.Equal("driver_number=1&driver_number=11&driver_number=44", builder.ToQueryString());
    }

    [Fact]
    public void Native_OrElse_on_the_same_equality_compared_field_repeats_the_key_like_WhereIn()
    {
        var builder = new FilterBuilder<DriversFilterFields>()
            .Where(x => x.DriverNumber == 1 || x.DriverNumber == 40);

        Assert.Equal("driver_number=1&driver_number=40", builder.ToQueryString());
    }

    [Fact]
    public void Native_OrElse_across_different_fields_throws()
    {
        var ex = Assert.Throws<NotSupportedException>(() =>
            new FilterBuilder<DriversFilterFields>().Where(x => x.DriverNumber == 1 || x.TeamName == "Ferrari"));

        Assert.Contains("only allows OR via repeating the SAME query key", ex.Message);
    }

    [Fact]
    public void Native_OrElse_with_a_non_equality_leaf_throws()
    {
        Assert.Throws<NotSupportedException>(() =>
            new FilterBuilder<LapsFilterFields>().Where(x => x.LapNumber == 1 || x.LapNumber > 5));
    }

    [Fact]
    public void Unsupported_operator_throws()
    {
        Assert.Throws<NotSupportedException>(() =>
            new FilterBuilder<DriversFilterFields>().Where(x => x.DriverNumber != 1));
    }
}
