using System.Text.Json;
using System.Text.Json.Serialization;
using OpenF1.Net.Filters;

namespace OpenF1.Net.Models.Enums;

/// <summary>The specific compound of tyre used during a stint.</summary>
[JsonConverter(typeof(TyreCompoundJsonConverter))]
public enum TyreCompound
{
    [ApiValue("SOFT")] Soft,
    [ApiValue("MEDIUM")] Medium,
    [ApiValue("HARD")] Hard,
    [ApiValue("INTERMEDIATE")] Intermediate,
    [ApiValue("WET")] Wet,
    [ApiValue("TEST_UNKNOWN")] TestUnknown,
    [ApiValue("UNKNOWN")] Unknown,
}

class TyreCompoundJsonConverter : JsonConverter<TyreCompound>
{
    static readonly Dictionary<string, TyreCompound> ApiValues = EnumApiValues.Build<TyreCompound>();

    public override TyreCompound Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var raw = reader.GetString()!;
        return ApiValues.TryGetValue(raw, out var compound)
            ? compound
            : throw new JsonException($"Unrecognized {nameof(TyreCompound)} value '{raw}' — the confirmed value set is closed, re-check the OpenF1 docs.");
    }

    public override void Write(Utf8JsonWriter writer, TyreCompound value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"{nameof(TyreCompound)} is response-only.");
}
