namespace OpenF1.Net.Models;

/// <summary>The weather over the track, updated every minute.</summary>
public class Weather
{
    /// <summary>Air temperature (°C).</summary>
    public double AirTemperature { get; init; }
    /// <summary>The UTC date and time, in ISO 8601 format.</summary>
    public DateTime Date { get; init; }
    /// <summary>Relative humidity (%).</summary>
    public double Humidity { get; init; }
    /// <summary>The unique identifier for the meeting. Use latest to identify the latest or current meeting.</summary>
    public int MeetingKey { get; init; }
    /// <summary>Air pressure (mbar).</summary>
    public double Pressure { get; init; }
    /// <summary>Whether there is rainfall.</summary>
    public double Rainfall { get; init; }
    /// <summary>The unique identifier for the session. Use latest to identify the latest or current session.</summary>
    public int SessionKey { get; init; }
    /// <summary>Track temperature (°C).</summary>
    public double TrackTemperature { get; init; }
    /// <summary>Wind direction (°), from 0° to 359°.</summary>
    public int WindDirection { get; init; }
    /// <summary>Wind speed (m/s).</summary>
    public double WindSpeed { get; init; }
}
