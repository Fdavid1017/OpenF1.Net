using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenF1.Net.Models;

/// <summary>
/// session_result.duration: the best/total lap time for race/practice, or the Q1/Q2/Q3 segment
/// durations for qualifying. Docs only ever describe float or array — no "+N LAP" string form here.
/// </summary>
[JsonConverter(typeof(SessionResultDurationJsonConverter))]
public class SessionResultDuration
{
    public double? Session { get; init; }
    public double? Q1 { get; init; }
    public double? Q2 { get; init; }
    public double? Q3 { get; init; }
}

class SessionResultDurationJsonConverter : JsonConverter<SessionResultDuration>
{
    public override SessionResultDuration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            double?[] segments = [null, null, null];
            var i = 0;
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if (i < 3)
                    segments[i] = reader.TokenType == JsonTokenType.Null ? null : reader.GetDouble();
                i++;
            }
            return new SessionResultDuration { Q1 = segments[0], Q2 = segments[1], Q3 = segments[2] };
        }

        return new SessionResultDuration { Session = reader.GetDouble() };
    }

    public override void Write(Utf8JsonWriter writer, SessionResultDuration value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"{nameof(SessionResultDuration)} is response-only.");
}
