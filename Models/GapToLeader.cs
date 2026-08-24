using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenF1.Net.Models;

/// <summary>
/// The time gap to the race leader/car ahead, in seconds — or "+N LAP(S)" text if the driver is lapped.
/// </summary>
[JsonConverter(typeof(GapToLeaderJsonConverter))]
public readonly struct GapToLeader
{
    public double? Seconds { get; }
    public string? LapsBehind { get; }

    public GapToLeader(double? seconds, string? lapsBehind)
    {
        Seconds = seconds;
        LapsBehind = lapsBehind;
    }

    public bool IsLapped => LapsBehind is not null;

    public override string ToString() => LapsBehind ?? $"{Seconds:0.000}s";
}

class GapToLeaderJsonConverter : JsonConverter<GapToLeader>
{
    public override GapToLeader Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        ReadScalar(ref reader);

    public override void Write(Utf8JsonWriter writer, GapToLeader value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"{nameof(GapToLeader)} is response-only.");

    internal static GapToLeader ReadScalar(ref Utf8JsonReader reader) =>
        reader.TokenType switch
        {
            JsonTokenType.String => new GapToLeader(null, reader.GetString()),
            _ => new GapToLeader(reader.GetDouble(), null),
        };
}
