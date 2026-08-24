using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenF1.Net.Models.Enums;

/// <summary>The Drag Reduction System (DRS) status, folded from the API's raw integer values.</summary>
[JsonConverter(typeof(DrsStatusJsonConverter))]
public enum DrsStatus
{
    /// <summary>Drs off</summary>
    Off,
    /// <summary>Detected, eligible once in activation zone</summary>
    Eligible,
    /// <summary>Drs on</summary>
    On,
}

class DrsStatusJsonConverter : JsonConverter<DrsStatus>
{
    public override DrsStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetInt32() switch
        {
            8 => DrsStatus.Eligible,
            10 or 12 or 14 => DrsStatus.On,
            _ => DrsStatus.Off, // 0, 1, 2, 3, 9, and any unrecognized future value
        };

    public override void Write(Utf8JsonWriter writer, DrsStatus value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"{nameof(DrsStatus)} is response-only.");
}
