using System.Reflection;

namespace OpenF1.Net.Filters;

/// <summary>Builds the raw-API-string → enum-member lookup used by every string-keyed enum's JsonConverter.</summary>
public static class EnumApiValues
{
    public static Dictionary<string, TEnum> Build<TEnum>() where TEnum : struct, Enum =>
        typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static)
            .ToDictionary(
                f => f.GetCustomAttribute<ApiValueAttribute>()?.Value ?? f.Name,
                f => (TEnum)f.GetValue(null)!);
}
