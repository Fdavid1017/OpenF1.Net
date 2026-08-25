using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace OpenF1.Net.Filters;

/// <summary>
/// Deferred, chainable, directly awaitable query returned by every OpenF1Client Get*Async method. No HTTP
/// call happens until the query is awaited, so .Where()/.And()/.WhereIn() can still be attached after
/// the call. Each endpoint's own Query type is a thin subclass — this base holds all the plumbing.
/// </summary>
public class EndpointQuery<TFields, TModel>
{
    readonly FilterBuilder<TFields> _builder = new();
    readonly Func<string, CancellationToken, Task<TModel[]>> _execute;
    readonly CancellationToken _ct;

    protected EndpointQuery(Func<string, CancellationToken, Task<TModel[]>> execute, CancellationToken ct)
    {
        _execute = execute;
        _ct = ct;
    }

    public EndpointQuery<TFields, TModel> Where(Expression<Func<TFields, bool>> predicate)
    {
        _builder.Where(predicate);
        return this;
    }

    public EndpointQuery<TFields, TModel> And(Expression<Func<TFields, bool>> predicate)
    {
        _builder.And(predicate);
        return this;
    }

    public EndpointQuery<TFields, TModel> WhereIn<TValue>(Expression<Func<TFields, TValue>> fieldSelector, params TValue[] values)
    {
        _builder.WhereIn(fieldSelector, values);
        return this;
    }

    protected CancellationToken CancellationToken => _ct;

    /// <summary>Runs the built query. Virtual so a subclass (e.g. MeetingsQuery) can layer post-processing on the result before it's awaited.</summary>
    protected virtual Task<TModel[]> ExecuteAsync() => _execute(_builder.ToQueryString(), _ct);

    public TaskAwaiter<TModel[]> GetAwaiter() => ExecuteAsync().GetAwaiter();
}
