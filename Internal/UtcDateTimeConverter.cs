using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenF1.Net.Internal;

/// <summary>
/// The API always returns dates with an explicit UTC offset. System.Text.Json's default DateTime
/// converter doesn't reliably normalize offset-bearing timestamps to Kind=Utc, so every date field
/// routes through this converter instead to guarantee a consistent Kind.
/// </summary>
internal class UtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        DateTime.Parse(
            reader.GetString()!,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
}
