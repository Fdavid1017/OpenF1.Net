namespace OpenF1.Net.Filters;

/// <summary>Maps an enum member to its raw OpenF1Client API string, when the two names diverge.</summary>
[AttributeUsage(AttributeTargets.Field)]
public class ApiValueAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}
