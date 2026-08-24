using OpenF1.Net.Tests.TestHelpers;

namespace OpenF1.Net.Tests.Unit;

public class WeatherTests
{
    [Fact]
    public async Task Deserializes_all_fields()
    {
        var (api, _) = MockHttpFactory.ForFixture("weather", "Weather.json");

        var data = await api.GetWeatherAsync();

        Assert.Single(data);
        var w = data[0];
        Assert.Equal(27.8, w.AirTemperature);
        Assert.Equal(58.0, w.Humidity);
        Assert.Equal(1219, w.MeetingKey);
        Assert.Equal(1010.1, w.Pressure);
        Assert.Equal(0, w.Rainfall);
        Assert.Equal(9161, w.SessionKey);
        Assert.Equal(34.5, w.TrackTemperature);
        Assert.Equal(180, w.WindDirection);
        Assert.Equal(1.6, w.WindSpeed);
    }
}
