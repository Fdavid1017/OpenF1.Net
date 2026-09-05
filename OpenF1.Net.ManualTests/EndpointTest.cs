using System.Diagnostics;
using OpenF1.Net.Filters;
using Spectre.Console;

namespace OpenF1.Net.ManualTests;

/// <summary>A single menu entry: one endpoint (or standalone client method) the harness can exercise.</summary>
public interface IEndpointTest
{
    /// <summary>The API path, used as the menu label and the saved file's name.</summary>
    string Endpoint { get; }

    /// <summary>The client method the entry calls, shown next to the endpoint in the menu.</summary>
    string Method { get; }

    /// <summary>Opens this entry's own screen and stays there until the user backs out.</summary>
    Task ShowAsync(OpenF1Client client, AppSettings settings, CancellationToken ct);
}

/// <summary>One opt-in the user can switch on before running a query, e.g. <c>.IncludeDriverDetails()</c>.</summary>
public record QueryOption<TFields, TModel>(string Label, Func<EndpointQuery<TFields, TModel>, EndpointQuery<TFields, TModel>> Apply);

/// <summary>
/// The screen behind every endpoint menu entry: build up filters and opt-ins, run, print. The filters and
/// selected opt-ins survive repeated runs so a query can be tweaked and re-fired without retyping it.
/// </summary>
public sealed class EndpointTest<TFields, TModel> : IEndpointTest where TFields : class
{
    public required string Endpoint { get; init; }
    public required string Method { get; init; }
    public required Func<OpenF1Client, CancellationToken, EndpointQuery<TFields, TModel>> CreateQuery { get; init; }
    public IReadOnlyList<QueryOption<TFields, TModel>> Options { get; init; } = [];

    readonly List<BuiltFilter<TFields>> _filters = [];
    readonly HashSet<string> _enabledOptions = [];

    public async Task ShowAsync(OpenF1Client client, AppSettings settings, CancellationToken ct)
    {
        const string run = "Run query";
        const string addFilter = "Add filter";
        const string clearFilters = "Clear filters";
        const string toggles = "Toggle options";
        const string back = "Back";

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[yellow]{Endpoint}[/] [grey]{Method}[/]").LeftJustified());
            AnsiConsole.MarkupLine($"[grey]Query:[/] {Markup.Escape(QueryPreview())}");
            if (Options.Count > 0)
                AnsiConsole.MarkupLine($"[grey]Options:[/] {Markup.Escape(_enabledOptions.Count == 0 ? "(none)" : string.Join(", ", _enabledOptions))}");
            AnsiConsole.WriteLine();

            var choices = new List<string> { run, addFilter };
            if (_filters.Count > 0)
                choices.Add(clearFilters);
            if (Options.Count > 0)
                choices.Add(toggles);
            choices.Add(back);

            switch (AnsiConsole.Prompt(new SelectionPrompt<string>().Title("What now?").AddChoices(choices)))
            {
                case run:
                    await RunAsync(client, settings, ct);
                    Output.Pause();
                    break;
                case addFilter:
                    var filter = FilterPrompt<TFields>.Prompt();
                    if (filter is not null)
                        _filters.Add(filter);
                    break;
                case clearFilters:
                    _filters.Clear();
                    break;
                case toggles:
                    PromptOptions();
                    break;
                default:
                    return;
            }
        }
    }

    void PromptOptions()
    {
        var prompt = new MultiSelectionPrompt<string>()
            .Title("Which options should the query be built with?")
            .NotRequired()
            .InstructionsText("[grey](space toggles, enter confirms)[/]")
            .AddChoices(Options.Select(o => o.Label));

        foreach (var label in Options.Select(o => o.Label).Where(_enabledOptions.Contains))
            prompt.Select(label);

        var selected = AnsiConsole.Prompt(prompt);
        _enabledOptions.Clear();
        foreach (var label in selected)
            _enabledOptions.Add(label);
    }

    string QueryPreview()
    {
        var clauses = string.Join("&", _filters.Select(f => f.Description));
        return $"GET /{Endpoint}{(clauses.Length == 0 ? "" : "?" + clauses)}";
    }

    async Task RunAsync(OpenF1Client client, AppSettings settings, CancellationToken ct)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[blue]{Markup.Escape(QueryPreview())}[/]");

        var query = CreateQuery(client, ct);
        foreach (var filter in _filters)
            query = query.Where(filter.Predicate);
        // Applied in declaration order, so an option that supersedes an earlier one (IncludeDriverDetails
        // with images vs without) wins by being declared last.
        foreach (var option in Options.Where(o => _enabledOptions.Contains(o.Label)))
            query = option.Apply(query);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var results = await query;
            stopwatch.Stop();
            Output.Write(Endpoint, results, stopwatch.Elapsed, settings);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Output.WriteError(ex, stopwatch.Elapsed);
        }
    }
}

/// <summary>A client method that isn't a filterable query — the two <c>latest</c> shortcuts.</summary>
public sealed class SingleCallTest : IEndpointTest
{
    public required string Endpoint { get; init; }
    public required string Method { get; init; }
    public required Func<OpenF1Client, CancellationToken, Task<object?>> Call { get; init; }

    public async Task ShowAsync(OpenF1Client client, AppSettings settings, CancellationToken ct)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[yellow]{Endpoint}[/] [grey]{Method}[/]").LeftJustified());

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await Call(client, ct);
            stopwatch.Stop();
            Output.WriteSingle(Endpoint, result, stopwatch.Elapsed, settings);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Output.WriteError(ex, stopwatch.Elapsed);
        }

        Output.Pause();
    }
}
