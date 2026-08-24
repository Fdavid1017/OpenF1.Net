using System.Text.Json;
using System.Text.Json.Serialization;
using OpenF1.Net.Filters;

namespace OpenF1.Net.Models.Enums;

/// <summary>
/// The type of the circuit. The docs only list "Permanent" and "Temporary - Street/Road", but live
/// data also returns the plain "Temporary - Street" (confirmed 2026-08-24, e.g. Singapore) — the docs
/// are stale here, same as the session_type/session_name mismatch noted elsewhere.
/// </summary>
[JsonConverter(typeof(CircuitTypeJsonConverter))]
public enum CircuitType
{
    [ApiValue("Permanent")] Permanent,
    [ApiValue("Temporary - Street/Road")] TemporaryStreetRoad,
    [ApiValue("Temporary - Street")] TemporaryStreet,
}

class CircuitTypeJsonConverter : JsonConverter<CircuitType>
{
    static readonly Dictionary<string, CircuitType> ApiValues = EnumApiValues.Build<CircuitType>();

    public override CircuitType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var raw = reader.GetString()!;
        return ApiValues.TryGetValue(raw, out var type)
            ? type
            : throw new JsonException($"Unrecognized {nameof(CircuitType)} value '{raw}' — the confirmed value set is closed, re-check the OpenF1 docs.");
    }

    public override void Write(Utf8JsonWriter writer, CircuitType value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"{nameof(CircuitType)} is response-only.");
}
