using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenF1.Net.Filters;

namespace OpenF1.Net.ManualTests;

/// <summary>
/// Renders a query result as the human-readable, API-shaped JSON the harness prints. The library's own
/// converters are response-only (their Write throws), so serializing has to route around them: enums go
/// out as their raw [ApiValue] API strings, and every OpenF1.Net.Models type is written by reflecting
/// over its public properties. Converters registered here win over the models' own [JsonConverter]
/// attributes, which is exactly what makes that possible.
/// </summary>
public static class ResultJson
{
    static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        // Results are read by a human, not re-parsed: don't escape '+', '<' and friends into \uXXXX escapes.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new ApiValueEnumConverterFactory(), new ModelWriteConverterFactory() },
    };

    public static string Serialize(object? value) => JsonSerializer.Serialize(value, Options);

    /// <summary>The first <paramref name="maxRows"/> rows of a result, as JSON — with a trailing "..." marker when truncated.</summary>
    public static string SerializeTruncated(Array rows, int maxRows)
    {
        if (maxRows <= 0 || rows.Length <= maxRows)
            return Serialize(rows);

        // No truncation marker inside the text: Spectre's JsonText parses what it renders, so the result
        // has to stay valid JSON. The caller says how many rows were dropped.
        return Serialize(rows.Cast<object?>().Take(maxRows).ToArray());
    }
}

/// <summary>Writes enums (and nullable enums) as the raw API string the wrapper parsed them from.</summary>
class ApiValueEnumConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => UnderlyingEnum(typeToConvert) is not null;

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var enumType = UnderlyingEnum(typeToConvert)!;
        var converterType = Nullable.GetUnderlyingType(typeToConvert) is null
            ? typeof(ApiValueEnumConverter<>).MakeGenericType(enumType)
            : typeof(NullableApiValueEnumConverter<>).MakeGenericType(enumType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    static Type? UnderlyingEnum(Type type)
    {
        var candidate = Nullable.GetUnderlyingType(type) ?? type;
        return candidate.IsEnum ? candidate : null;
    }

    internal static string ToApiString<TEnum>(TEnum value) where TEnum : struct, Enum =>
        typeof(TEnum).GetField(value.ToString())?.GetCustomAttribute<ApiValueAttribute>()?.Value ?? value.ToString();
}

class ApiValueEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException("The harness only ever writes results.");

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options) =>
        writer.WriteStringValue(ApiValueEnumConverterFactory.ToApiString(value));
}

class NullableApiValueEnumConverter<TEnum> : JsonConverter<TEnum?> where TEnum : struct, Enum
{
    public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException("The harness only ever writes results.");

    public override void Write(Utf8JsonWriter writer, TEnum? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(ApiValueEnumConverterFactory.ToApiString(value.Value));
    }
}

/// <summary>
/// Writes any OpenF1.Net.Models type — class or struct — by reflecting over its public properties,
/// bypassing the response-only converters several of those models carry as attributes. Arrays and other
/// sequences are deliberately left to System.Text.Json, which already writes them as JSON arrays.
/// </summary>
class ModelWriteConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        var type = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
        if (type == typeof(string) || type.IsEnum || type.IsArray || typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
            return false;

        return type.Namespace?.StartsWith("OpenF1.Net.Models", StringComparison.Ordinal) == true;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var underlying = Nullable.GetUnderlyingType(typeToConvert);
        var converterType = underlying is null
            ? typeof(ModelWriteConverter<>).MakeGenericType(typeToConvert)
            : typeof(NullableModelWriteConverter<>).MakeGenericType(underlying);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

static class ModelProperties
{
    public static void Write<T>(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var property in Cache<T>.Properties)
        {
            writer.WritePropertyName(PropertyName(property, options));
            JsonSerializer.Serialize(writer, property.GetValue(value), property.PropertyType, options);
        }

        writer.WriteEndObject();
    }

    // A model that spells its API field name out via [JsonPropertyName] (duration_sector_1 and friends,
    // where the naming policy's conversion would be wrong) keeps that name here too.
    static string PropertyName(PropertyInfo property, JsonSerializerOptions options) =>
        property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
        ?? options.PropertyNamingPolicy?.ConvertName(property.Name)
        ?? property.Name;

    static class Cache<T>
    {
        public static readonly PropertyInfo[] Properties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetIndexParameters().Length == 0 && p.CanRead)
            .OrderBy(p => p.Name, StringComparer.Ordinal)
            .ToArray();
    }
}

class ModelWriteConverter<T> : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException("The harness only ever writes results.");

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
        ModelProperties.Write(writer, value, options);
}

class NullableModelWriteConverter<T> : JsonConverter<T?> where T : struct
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException("The harness only ever writes results.");

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            ModelProperties.Write(writer, value.Value, options);
    }
}
