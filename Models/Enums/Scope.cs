using System.Text.Json;
using System.Text.Json.Serialization;
using OpenF1.Net.Filters;

namespace OpenF1.Net.Models.Enums;

/// <summary>
/// The scope of a race_control event (Track, Sector, Driver). Its converter is registered explicitly
/// in OpenF1's JsonSerializerOptions rather than via [JsonConverter] here — see the note on Flag.
/// </summary>
public enum Scope
{
    [ApiValue("Track")] Track,
    [ApiValue("Sector")] Sector,
    [ApiValue("Driver")] Driver,
}

/// <summary>
/// race_control rows outside category Flag/Drs carry no scope value, and the confirmed value set is
/// closed — so both "field absent" and "unrecognized future value" map to null.
/// </summary>
class NullableScopeJsonConverter : JsonConverter<Scope?>
{
    static readonly Dictionary<string, Scope> ApiValues = EnumApiValues.Build<Scope>();

    public override Scope? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        return ApiValues.TryGetValue(reader.GetString()!, out var scope) ? scope : null;
    }

    public override void Write(Utf8JsonWriter writer, Scope? value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"{nameof(Scope)} is response-only.");
}
