namespace OpenF1.Net.Filters;

/// <summary>
/// Overrides FilterBuilder's default PascalCase-to-snake_case field-name conversion for a *FilterFields
/// property, when that conversion doesn't match the API's actual field name — e.g. a trailing numbered
/// suffix like duration_sector_1, where the default conversion drops the underscore before the digit.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ApiFieldNameAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}
