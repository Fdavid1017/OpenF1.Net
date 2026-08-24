using OpenF1.Net.Models;

namespace OpenF1.Net.Filters;

public class WeatherFilterFields
{
    public double AirTemperature => throw new NotSupportedException();
    public DateTime Date => throw new NotSupportedException();
    public double Humidity => throw new NotSupportedException();
    public MeetingKeyRef MeetingKey => throw new NotSupportedException();
    public double Pressure => throw new NotSupportedException();
    public double Rainfall => throw new NotSupportedException();
    public SessionKeyRef SessionKey => throw new NotSupportedException();
    public double TrackTemperature => throw new NotSupportedException();
    public int WindDirection => throw new NotSupportedException();
    public double WindSpeed => throw new NotSupportedException();
}

public class WeatherQuery(Func<string, CancellationToken, Task<Weather[]>> execute, CancellationToken ct)
    : EndpointQuery<WeatherFilterFields, Weather>(execute, ct);
