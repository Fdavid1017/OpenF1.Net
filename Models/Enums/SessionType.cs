using System.Text.Json;
using System.Text.Json.Serialization;
using OpenF1.Net.Filters;

namespace OpenF1.Net.Models.Enums;

/// <summary>
/// The type of the session. Sprint/Sprint Qualifying session_names map to Race/Qualifying
/// respectively, not a distinct type — confirmed live against the API; the docs' own example is stale.
/// </summary>
[JsonConverter(typeof(SessionTypeJsonConverter))]
public enum SessionType
{
    [ApiValue("Practice")] Practice,
    [ApiValue("Qualifying")] Qualifying,
    [ApiValue("Race")] Race,
}

class SessionTypeJsonConverter : JsonConverter<SessionType>
{
    static readonly Dictionary<string, SessionType> ApiValues = EnumApiValues.Build<SessionType>();

    public override SessionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var raw = reader.GetString()!;
        return ApiValues.TryGetValue(raw, out var type)
            ? type
            : throw new JsonException($"Unrecognized {nameof(SessionType)} value '{raw}' — the confirmed value set is closed, re-check the OpenF1 docs.");
    }

    public override void Write(Utf8JsonWriter writer, SessionType value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"{nameof(SessionType)} is response-only.");
}
