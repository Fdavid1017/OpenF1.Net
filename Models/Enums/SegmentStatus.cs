using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenF1.Net.Models.Enums;

/// <summary>A "mini-sector" status value within laps.segments_sector_*.</summary>
[JsonConverter(typeof(SegmentStatusJsonConverter))]
public enum SegmentStatus
{
    Unavailable,
    Yellow,
    Green,
    Purple,
    Pitlane,
    /// <summary>
    /// Covers raw values 2050/2052/2068 (the OpenF1 docs mark these "?" themselves) and any other
    /// unrecognized future value.
    /// </summary>
    Unknown,
}

class SegmentStatusJsonConverter : JsonConverter<SegmentStatus>
{
    public override SegmentStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Live data includes null entries (segment not yet reached) alongside the docs' raw integer codes.
        if (reader.TokenType == JsonTokenType.Null)
            return SegmentStatus.Unavailable;

        return reader.GetInt32() switch
        {
            0 => SegmentStatus.Unavailable,
            2048 => SegmentStatus.Yellow,
            2049 => SegmentStatus.Green,
            2051 => SegmentStatus.Purple,
            2064 => SegmentStatus.Pitlane,
            _ => SegmentStatus.Unknown,
        };
    }

    public override void Write(Utf8JsonWriter writer, SegmentStatus value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"{nameof(SegmentStatus)} is response-only.");
}
