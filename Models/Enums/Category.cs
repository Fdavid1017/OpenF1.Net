using System.Text.Json;
using System.Text.Json.Serialization;
using OpenF1.Net.Filters;

namespace OpenF1.Net.Models.Enums;

/// <summary>The category of a race_control event (SessionStatus, CarEvent, Drs, Flag, SafetyCar, ...).</summary>
[JsonConverter(typeof(CategoryJsonConverter))]
public enum Category
{
    [ApiValue("SessionStatus")] SessionStatus,
    [ApiValue("CarEvent")] CarEvent,
    [ApiValue("Drs")] Drs,
    [ApiValue("Flag")] Flag,
    [ApiValue("SafetyCar")] SafetyCar,
    /// <summary>Catch-all — also the fallback for any unrecognized future value.</summary>
    [ApiValue("Other")] Other,
}

class CategoryJsonConverter : JsonConverter<Category>
{
    static readonly Dictionary<string, Category> ApiValues = EnumApiValues.Build<Category>();

    public override Category Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        ApiValues.TryGetValue(reader.GetString()!, out var category) ? category : Category.Other;

    public override void Write(Utf8JsonWriter writer, Category value, JsonSerializerOptions options) =>
        throw new NotSupportedException($"{nameof(Category)} is response-only.");
}
