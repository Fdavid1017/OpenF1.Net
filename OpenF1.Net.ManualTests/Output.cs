using System.Globalization;
using OpenF1.Net.Exceptions;
using Spectre.Console;
using Spectre.Console.Json;

namespace OpenF1.Net.ManualTests;

/// <summary>Prints a run's outcome: row count, timing, the JSON itself, and (optionally) the saved file.</summary>
public static class Output
{
    public static void Write(string endpoint, Array results, TimeSpan elapsed, AppSettings settings)
    {
        AnsiConsole.MarkupLine($"[green]{results.Length} row(s)[/] in [green]{elapsed.TotalMilliseconds:F0} ms[/]");
        AnsiConsole.WriteLine();

        if (results.Length == 0)
        {
            AnsiConsole.WriteLine("[]");
            return;
        }

        var printed = settings.MaxRowsPrinted;
        var truncated = printed > 0 && results.Length > printed;
        if (truncated)
            AnsiConsole.MarkupLine($"[grey]Showing the first {printed} of {results.Length} rows (Settings -> rows printed).[/]");

        AnsiConsole.Write(new JsonText(ResultJson.SerializeTruncated(results, printed)));
        AnsiConsole.WriteLine();
        if (truncated)
            AnsiConsole.MarkupLine($"[grey]... {results.Length - printed} more row(s) not shown.[/]");
        SaveIfEnabled(endpoint, ResultJson.Serialize(results), settings);
    }

    public static void WriteSingle(string endpoint, object? result, TimeSpan elapsed, AppSettings settings)
    {
        AnsiConsole.MarkupLine($"[green]done[/] in [green]{elapsed.TotalMilliseconds:F0} ms[/]");
        AnsiConsole.WriteLine();

        var json = ResultJson.Serialize(result);
        AnsiConsole.Write(new JsonText(json));
        AnsiConsole.WriteLine();
        SaveIfEnabled(endpoint, json, settings);
    }

    public static void WriteError(Exception ex, TimeSpan elapsed)
    {
        var label = ex switch
        {
            OpenF1RateLimitExceededException => "rate limit exceeded",
            OpenF1SubscriptionRequiredException => "subscription required",
            OpenF1ApiException api => $"API error ({(int)api.StatusCode})",
            OperationCanceledException => "cancelled",
            _ => ex.GetType().Name,
        };

        AnsiConsole.MarkupLine($"[red]{Markup.Escape(label)}[/] after [grey]{elapsed.TotalMilliseconds:F0} ms[/]");
        AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
    }

    /// <summary>
    /// Waits for a real keypress before the caller repaints the screen. Spectre's prompts can leave a
    /// key record (the Enter that confirmed the selection, or a key-up) in the console buffer, which a
    /// bare ReadKey would consume instantly — so the buffer is drained first.
    /// </summary>
    public static void Pause(string message = "Press any key to return...")
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[grey]{Markup.Escape(message)}[/]");

        try
        {
            while (Console.KeyAvailable)
                Console.ReadKey(intercept: true);
        }
        catch (InvalidOperationException)
        {
            // Input is redirected (no interactive console); nothing to drain and nothing to wait for.
            return;
        }

        Console.ReadKey(intercept: true);
    }

    static void SaveIfEnabled(string endpoint, string json, AppSettings settings)
    {
        if (!settings.SaveResults)
            return;

        try
        {
            Directory.CreateDirectory(settings.OutputDirectory);
            var name = $"{endpoint}_{DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)}.json";
            var path = Path.Combine(settings.OutputDirectory, name);
            File.WriteAllText(path, json);
            AnsiConsole.MarkupLine($"[grey]Full result saved to[/] {Markup.Escape(path)}");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Could not save the result:[/] {Markup.Escape(ex.Message)}");
        }
    }
}
