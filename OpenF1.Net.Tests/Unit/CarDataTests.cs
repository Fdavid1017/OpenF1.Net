using OpenF1.Net.Models.Enums;
using OpenF1.Net.Tests.TestHelpers;
using RichardSzalay.MockHttp;

namespace OpenF1.Net.Tests.Unit;

public class CarDataTests
{
    [Fact]
    public async Task Deserializes_all_fields_and_folds_drs_status()
    {
        var (api, _) = MockHttpFactory.ForFixture("car_data", "CarData.json");

        var data = await api.GetCarDataAsync();

        Assert.Equal(3, data.Length);
        var first = data[0];
        Assert.Equal(0, first.Brake);
        Assert.Equal(new DateTime(2023, 9, 15, 13, 8, 19, 923, DateTimeKind.Utc), first.Date);
        Assert.Equal(DateTimeKind.Utc, first.Date.Kind);
        Assert.Equal(55, first.DriverNumber);
        Assert.Equal(DrsStatus.On, first.Drs);
        Assert.Equal(1219, first.MeetingKey);
        Assert.Equal(8, first.NGear);
        Assert.Equal(11141, first.Rpm);
        Assert.Equal(9159, first.SessionKey);
        Assert.Equal(315, first.Speed);
        Assert.Equal(100, first.Throttle);

        Assert.Equal(DrsStatus.Eligible, data[1].Drs);
        Assert.Equal(DrsStatus.Off, data[2].Drs);
    }

    [Fact]
    public async Task IncludeDriverDetails_attaches_the_matching_driver_and_dedups_by_session_and_driver_number()
    {
        var (api, mockHttp) = MockHttpFactory.ForFixture("car_data", "CarData.json");
        const string sainzOnly = """[{"driver_number":55,"last_name":"Sainz","session_key":9159,"meeting_key":1219}]""";
        // All three fixture rows share driver_number=55/session_key=9159 — only one /drivers call should happen for them.
        var driversRequest = mockHttp.When("https://api.openf1.org/v1/drivers?session_key=9159&driver_number=55").Respond("application/json", sainzOnly);

        var data = await api.GetCarDataAsync().IncludeDriverDetails();

        Assert.Equal(3, data.Length);
        Assert.All(
            data,
            d =>
            {
                Assert.NotNull(d.DriverDetails);
                Assert.Equal(55, d.DriverDetails!.DriverNumber);
                Assert.Equal("Sainz", d.DriverDetails.LastName);
            }
        );
        Assert.Equal(1, mockHttp.GetMatchCount(driversRequest));
    }
}
