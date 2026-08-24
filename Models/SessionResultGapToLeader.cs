using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenF1.Net.Models;

/// <summary>
/// session_result.gap_to_leader: a scalar gap for race/practice sessions, or the Q1/Q2/Q3 segment
/// gaps for qualifying. Never both — exactly one shape is populated per row.
/// </summary>
[JsonConverter(typeof(SessionResultGapToLeaderJsonConverter))]
public class SessionResultGapToLeader
{
    public GapToLeader? Session { get; init; }
    public double? Q1 { get; init; }
    public double? Q2 { get; init; }
    public double? Q3 { get; init; }
}

class SessionResultGapToLeaderJsonConverter : JsonConverter<SessionResultGapToLeader>
{
    public override SessionResultGapToLeader Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            return new SessionResultGapToLeader { Q1 = segments[0], Q2 = segments[1], Q3 = segments[2] };
        }

        return new SessionResultGapToLeader { Session = GapToLeaderJsonConverter.ReadScalar(ref reader) };
    }

    public override void Write(Utf8JsonWriter writer, SessionResultGapToLeader value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"{nameof(SessionResultGapToLeader)} is response-only.");
}
