using OpenF1.Net.Models.Enums;
using OpenF1.Net.Tests.TestHelpers;

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
}
