using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using OpenF1.Net.Filters;
using Spectre.Console;

namespace OpenF1.Net.ManualTests;

/// <summary>One filter clause the user built interactively, ready to hand to a query.</summary>
/// <param name="Description">The clause as the API would express it, e.g. <c>session_key=latest</c>.</param>
public record BuiltFilter<TFields>(string Description, Expression<Func<TFields, bool>> Predicate);

/// <summary>
/// Turns a *FilterFields marker class into an interactive filter builder. The fields, their types and the
/// operators each type allows are all read off the marker class by reflection, so a filterable field added
/// to the library shows up here without any change to the harness.
/// </summary>
public static class FilterPrompt<TFields> where TFields : class
{
    static readonly PropertyInfo[] Fields = typeof(TFields)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .OrderBy(p => p.Name, StringComparer.Ordinal)
        .ToArray();

    public static BuiltFilter<TFields>? Prompt()
    {
        const string cancel = "(cancel)";
        var choices = Fields.Select(f => $"{ApiFieldName(f)}  [grey]({TypeLabel(f.PropertyType)})[/]").ToList();
        choices.Add(cancel);

        var chosen = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Which field do you want to filter on?")
                .PageSize(20)
                .MoreChoicesText("[grey](move up and down to see more fields)[/]")
                .AddChoices(choices));

        if (chosen == cancel)
            return null;

        var field = Fields[choices.IndexOf(chosen)];
        return Build(field, PromptOperator(field.PropertyType), PromptValue(field.PropertyType));
    }

    /// <summary>Assembles one clause from an already-chosen field, operator and value.</summary>
    public static BuiltFilter<TFields> Build(PropertyInfo field, string op, object value)
    {
        var parameter = Expression.Parameter(typeof(TFields), "f");
        var member = Expression.Property(parameter, field);
        var body = BuildComparison(member, op, value, field.PropertyType);
        var predicate = Expression.Lambda<Func<TFields, bool>>(body, parameter);

        return new BuiltFilter<TFields>($"{ApiFieldName(field)}{op}{Describe(value)}", predicate);
    }

    static string PromptOperator(Type type)
    {
        var operators = SupportsOrdering(type) ? new[] { "=", ">", ">=", "<", "<=" } : ["="];
        return operators.Length == 1
            ? operators[0]
            : AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Operator?").AddChoices(operators));
    }

    // The API only orders values that are actually ordered; everything else is an equality match.
    static bool SupportsOrdering(Type type) =>
        type == typeof(int) || type == typeof(double) || type == typeof(DateTime);

    static object PromptValue(Type type)
    {
        if (type == typeof(SessionKeyRef) || type == typeof(MeetingKeyRef))
        {
            var isLatest = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Value?").AddChoices("latest", "a specific key")) == "latest";
            if (isLatest)
                return type == typeof(SessionKeyRef) ? SessionKeyRef.Latest : MeetingKeyRef.Latest;

            var key = AnsiConsole.Prompt(new TextPrompt<int>("Key:"));
            return type == typeof(SessionKeyRef) ? (SessionKeyRef)key : (MeetingKeyRef)key;
        }

        if (type.IsEnum)
        {
            var members = Enum.GetValues(type).Cast<object>().ToArray();
            var labels = members.Select(EnumLabel).ToArray();
            var picked = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Value?").PageSize(20).AddChoices(labels));
            return members[Array.IndexOf(labels, picked)];
        }

        if (type == typeof(bool))
            return AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Value?").AddChoices("true", "false")) == "true";

        if (type == typeof(DateTime))
        {
            var raw = AnsiConsole.Prompt(
                new TextPrompt<string>("Value [grey](UTC, e.g. 2023-09-15T13:00:00)[/]:")
                    .Validate(candidate => TryParseUtc(candidate, out _)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Not a date/time — try 2023-09-15T13:00:00[/]")));
            TryParseUtc(raw, out var parsed);
            return parsed;
        }

        if (type == typeof(int))
            return AnsiConsole.Prompt(new TextPrompt<int>("Value:"));

        if (type == typeof(double))
            return AnsiConsole.Prompt(new TextPrompt<double>("Value:"));

        return AnsiConsole.Prompt(new TextPrompt<string>("Value:"));
    }

    static bool TryParseUtc(string raw, out DateTime value) =>
        DateTime.TryParse(
            raw,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
            out value);

    /// <summary>
    /// Builds the same expression shape the C# compiler would have produced for a hand-written filter, which
    /// is what FilterBuilder reads: enum comparisons arrive as Convert(field, int) == &lt;int constant&gt;,
    /// everything else as a plain field-to-constant comparison.
    /// </summary>
    static Expression BuildComparison(MemberExpression member, string op, object value, Type type)
    {
        Expression left = member;
        Expression right;
        if (type.IsEnum)
        {
            var underlying = Enum.GetUnderlyingType(type);
            left = Expression.Convert(member, underlying);
            right = Expression.Constant(Convert.ChangeType(value, underlying, CultureInfo.InvariantCulture), underlying);
        }
        else
        {
            right = Expression.Constant(value, type);
        }

        return op switch
        {
            "=" => Expression.Equal(left, right),
            ">" => Expression.GreaterThan(left, right),
            ">=" => Expression.GreaterThanOrEqual(left, right),
            "<" => Expression.LessThan(left, right),
            "<=" => Expression.LessThanOrEqual(left, right),
            _ => throw new NotSupportedException($"Unknown operator '{op}'."),
        };
    }

    static string Describe(object value) => value switch
    {
        DateTime dt => dt.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
        bool b => b ? "true" : "false",
        Enum e => EnumLabel(e),
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? "",
    };

    static string EnumLabel(object value) =>
        value.GetType().GetField(value.ToString()!)?.GetCustomAttribute<ApiValueAttribute>()?.Value ?? value.ToString()!;

    static string TypeLabel(Type type) => type switch
    {
        _ when type == typeof(SessionKeyRef) || type == typeof(MeetingKeyRef) => "int or latest",
        _ when type.IsEnum => "enum",
        _ => type.Name.ToLowerInvariant(),
    };

    // Mirrors FilterBuilder's own PascalCase-to-snake_case conversion, including the [ApiFieldName] override.
    static string ApiFieldName(PropertyInfo property) =>
        property.GetCustomAttribute<ApiFieldNameAttribute>()?.Value
        ?? string.Concat(property.Name.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())).ToLowerInvariant();
}
