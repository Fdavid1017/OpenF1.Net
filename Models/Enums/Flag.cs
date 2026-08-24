using System.Text.Json;
using System.Text.Json.Serialization;
using OpenF1.Net.Filters;

namespace OpenF1.Net.Models.Enums;

/// <summary>
/// Type of flag displayed (GREEN, YELLOW, DOUBLE YELLOW, CHEQUERED, ...). Its converter is registered
/// explicitly in OpenF1's JsonSerializerOptions rather than via [JsonConverter] here — Nullable&lt;T&gt;
/// resolution always looks up a converter for the plain T first, so a converter attributed here would
/// need to target Flag rather than Flag?, which can't express the "unrecognized value -&gt; null" fallback.
/// </summary>
public enum Flag
{
    [ApiValue("GREEN")] Green,
    [ApiValue("DOUBLE YELLOW")] DoubleYellow,
    [ApiValue("YELLOW")] Yellow,
    [ApiValue("RED")] Red,
    [ApiValue("CLEAR")] Clear,
    [ApiValue("BLUE")] Blue,
    [ApiValue("CHEQUERED")] Chequered,
    [ApiValue("BLACK AND WHITE")] BlackAndWhite,
}

/// <summary>
/// race_control rows outside category Flag carry no flag value, and the confirmed value set is
/// closed — so both "field absent" and "unrecognized future value" map to null rather than a
/// synthetic enum member.
/// </summary>
class NullableFlagJsonConverter : JsonConverter<Flag?>
{
    static readonly Dictionary<string, Flag> ApiValues = EnumApiValues.Build<Flag>();

    public override Flag? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        return ApiValues.TryGetValue(reader.GetString()!, out var flag) ? flag : null;
    }

    public override void Write(Utf8JsonWriter writer, Flag? value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"{nameof(Flag)} is response-only.");
}
