using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace OpenF1.Net.Filters;

/// <summary>
/// Parses filter expressions into the OpenF1 API's query-string grammar. TFields is a per-endpoint
/// marker class whose properties (never actually invoked — only the expression tree shape is read)
/// declare which response fields are filterable.
/// </summary>
public class FilterBuilder<TFields>
{
    readonly List<string> _clauses = [];

    public FilterBuilder<TFields> Where(Expression<Func<TFields, bool>> predicate) => And(predicate);

    public FilterBuilder<TFields> And(Expression<Func<TFields, bool>> predicate)
    {
        _clauses.Add(ParseClause(predicate.Body));
        return this;
    }

    // OR is only expressible via the API as repeated same-key params — no arbitrary boolean OR across fields.
    public FilterBuilder<TFields> WhereIn<TValue>(Expression<Func<TFields, TValue>> fieldSelector, params TValue[] values)
    {
        var member = (MemberExpression)fieldSelector.Body;
        var fieldName = ToSnakeCase(member.Member.Name);
        foreach (var v in values)
            _clauses.Add($"{fieldName}={FormatValue(v)}");
        return this;
    }

    public string ToQueryString() => string.Join("&", _clauses);

    static string ParseClause(Expression body)
    {
        if (body is BinaryExpression { NodeType: ExpressionType.OrElse } orElse)
            return ParseOr(orElse);

        if (body is not BinaryExpression binary)
            throw new NotSupportedException($"Only direct field comparisons are supported, got: {body}");

        var op = binary.NodeType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            _ => throw new NotSupportedException(
                $"Operator '{binary.NodeType}' isn't in the API's supported set (=, <, >, <=, >=)."),
        };

        // Enum comparisons get wrapped by the compiler as Convert(x.Field, int) == Convert(Enum.X, int) — unwrap it.
        var left = binary.Left is UnaryExpression { NodeType: ExpressionType.Convert } unary ? unary.Operand : binary.Left;
        var member = left as MemberExpression
            ?? throw new NotSupportedException("Left side of a filter comparison must be a field access.");
        var fieldName = ToSnakeCase(member.Member.Name);

        var right = binary.Right is UnaryExpression { NodeType: ExpressionType.Convert } rightUnary ? rightUnary.Operand : binary.Right;
        var value = Expression.Lambda(right).Compile().DynamicInvoke();
        // The compiler constant-folds an enum literal on the right into its bare underlying int, dropping
        // the enum type entirely — recover it from the left side's declared property type (member.Type).
        if (member.Type.IsEnum && value is not null)
            value = Enum.ToObject(member.Type, value);
        return $"{fieldName}{op}{FormatValue(value)}";
    }

    // || is only meaningful to the API as repeated same-key params, same as WhereIn — so || across
    // different fields is rejected loudly instead of silently producing a query the API can't honor.
    static string ParseOr(BinaryExpression orElse)
    {
        var leaves = new List<BinaryExpression>();
        CollectOrLeaves(orElse, leaves);

        string? fieldName = null;
        var clauses = new List<string>();
        foreach (var leaf in leaves)
        {
            if (leaf.NodeType != ExpressionType.Equal)
                throw new NotSupportedException("'||' is only supported for equality checks (==) — the API expresses OR as a repeated '=' param.");

            var left = leaf.Left is UnaryExpression { NodeType: ExpressionType.Convert } lu ? lu.Operand : leaf.Left;
            var member = left as MemberExpression
                ?? throw new NotSupportedException("Left side of an '||' comparison must be a field access.");
            var name = ToSnakeCase(member.Member.Name);
            if (fieldName is null)
                fieldName = name;
            else if (fieldName != name)
                throw new NotSupportedException(
                    $"'||' across different fields ('{fieldName}' vs '{name}') isn't supported — the OpenF1 API only allows OR via repeating the SAME query key, e.g. driver_number=1&driver_number=40.");

            var right = leaf.Right is UnaryExpression { NodeType: ExpressionType.Convert } ru ? ru.Operand : leaf.Right;
            var value = Expression.Lambda(right).Compile().DynamicInvoke();
            if (member.Type.IsEnum && value is not null)
                value = Enum.ToObject(member.Type, value);
            clauses.Add($"{name}={FormatValue(value)}");
        }
        return string.Join("&", clauses);
    }

    static void CollectOrLeaves(Expression expr, List<BinaryExpression> leaves)
    {
        switch (expr)
        {
            case BinaryExpression { NodeType: ExpressionType.OrElse } orElse:
                CollectOrLeaves(orElse.Left, leaves);
                CollectOrLeaves(orElse.Right, leaves);
                break;
            case BinaryExpression be:
                leaves.Add(be);
                break;
            default:
                throw new NotSupportedException($"Unsupported expression inside '||': {expr}");
        }
    }

    static string FormatValue(object? value) => value switch
    {
        DateTime dt => dt.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
        bool b => b ? "true" : "false",
        SessionKeyRef s => s.ToString(),
        MeetingKeyRef m => m.ToString(),
        Enum e => GetApiValue(e),
        null => "",
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? "",
    };

    // Enum members serialize back to the API's raw string via [ApiValue], not the C# member name
    // (e.g. Flag.BlackAndWhite -> "BLACK AND WHITE"). A real HTTP call still needs Uri.EscapeDataString
    // on top of this for values containing spaces — ToQueryString() here produces the pre-encoding form.
    static string GetApiValue(Enum e) =>
        e.GetType().GetField(e.ToString())!.GetCustomAttribute<ApiValueAttribute>()?.Value ?? e.ToString();

    static string ToSnakeCase(string pascal) =>
        string.Concat(pascal.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())).ToLowerInvariant();
}
