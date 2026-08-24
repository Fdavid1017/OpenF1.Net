using System.Text.Json;
using System.Text.Json.Serialization;
using OpenF1.Net.Filters;

namespace OpenF1.Net.Models.Enums;

/// <summary>The name of the session (Practice 1, Qualifying, Race, Sprint, ...).</summary>
[JsonConverter(typeof(SessionNameJsonConverter))]
public enum SessionName
{
    [ApiValue("Day 1")] Day1,
    [ApiValue("Day 2")] Day2,
    [ApiValue("Day 3")] Day3,
    [ApiValue("Practice 1")] Practice1,
    [ApiValue("Practice 2")] Practice2,
    [ApiValue("Practice 3")] Practice3,
    [ApiValue("Qualifying")] Qualifying,
    [ApiValue("Race")] Race,
    [ApiValue("Sprint Qualifying")] SprintQualifying,
    [ApiValue("Sprint")] Sprint,
}

class SessionNameJsonConverter : JsonConverter<SessionName>
{
    static readonly Dictionary<string, SessionName> ApiValues = EnumApiValues.Build<SessionName>();

    public override SessionName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var raw = reader.GetString()!;
        return ApiValues.TryGetValue(raw, out var name)
            ? name
            : throw new JsonException($"Unrecognized {nameof(SessionName)} value '{raw}' — the confirmed value set is closed, re-check the OpenF1 docs.");
    }

    public override void Write(Utf8JsonWriter writer, SessionName value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"{nameof(SessionName)} is response-only.");
}
