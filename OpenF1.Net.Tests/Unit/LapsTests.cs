using System.Net;
using OpenF1.Net.Models.Enums;
using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

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

    [Fact]
    public async Task Without_IncludeDriverDetails_DriverDetails_stays_null()
    {
        var (api, _) = MockHttpFactory.ForFixture("laps", "Laps.json");

        var data = await api.GetLapsAsync();

        Assert.All(data, l => Assert.Null(l.DriverDetails));
    }

    [Fact]
    public async Task IncludeDriverDetails_attaches_the_matching_driver_and_dedups_by_session_and_driver_number()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("laps", "Laps.json");
        // The real API would filter server-side to just Verstappen for this querystring — mirror that here
        // rather than returning the full (unfiltered) Drivers.json fixture.
        const string verstappenOnly = """[{"driver_number":1,"last_name":"Verstappen","session_key":9161,"meeting_key":1219}]""";
        // Both fixture laps share driver_number=1/session_key=9161 — only one /drivers call should happen for the two of them.
        var driversRequest = mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=1").Respond("application/json", verstappenOnly);

        var data = await api.GetLapsAsync().IncludeDriverDetails();

        Assert.Equal(2, data.Length);
        Assert.All(
            data,
            l =>
            {
                Assert.NotNull(l.DriverDetails);
                Assert.Equal(1, l.DriverDetails!.DriverNumber);
                Assert.Equal("Verstappen", l.DriverDetails.LastName);
            }
        );
        Assert.Equal(1, mockHttp.GetMatchCount(driversRequest));
    }

    [Fact]
    public async Task IncludeDriverDetails_with_resolveImages_true_also_resolves_the_attached_drivers_images()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("laps", "Laps.json");
        mockHttp.Fallback.Respond(HttpStatusCode.NotFound);
        const string verstappenOnly =
            """[{"driver_number":1,"first_name":"Max","last_name":"Verstappen","name_acronym":"VER","team_name":"Red Bull Racing","session_key":9161,"meeting_key":1219}]""";
        mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=1").Respond("application/json", verstappenOnly);
        mockHttp
            .When("https://media.formula1.com/content/dam/fom-website/2018-redesign-assets/drivers/2023/MAXVER01.png")
            .Respond(HttpStatusCode.OK);

        var data = await api.GetLapsAsync().IncludeDriverDetails(resolveImages: true);

        Assert.All(
            data,
            l =>
                Assert.Equal(
                    "https://media.formula1.com/content/dam/fom-website/2018-redesign-assets/drivers/2023/MAXVER01.png",
                    l.DriverDetails!.HeadshotUrl
                )
        );
    }

    [Fact]
    public async Task IncludeDriverDetails_leaves_DriverDetails_null_when_no_driver_matches_and_still_dedups()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("laps", "Laps.json");
        var driversRequest = mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9161&driver_number=1").Respond("application/json", "[]");

        var data = await api.GetLapsAsync().IncludeDriverDetails();

        Assert.Equal(2, data.Length);
        Assert.All(data, l => Assert.Null(l.DriverDetails));
        Assert.Equal(1, mockHttp.GetMatchCount(driversRequest));
    }
}
