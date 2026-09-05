using OpenF1.Net;
using OpenF1.Net.ManualTests;
using Spectre.Console;

// Manual, interactive harness for OpenF1.Net: pick an endpoint, switch on the opt-ins that endpoint
// offers, add filters, fire the real API, and read the result back as formatted JSON.

var settings = AppSettings.Load();
var tests = Catalog.Build();

using var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cancellation.Cancel();
};

var client = NewClient(settings);

try
{
    while (true)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("OpenF1.Net").Color(Color.Red));
        AnsiConsole.MarkupLine("[grey]Manual test harness - every call hits the real https://api.openf1.org/v1[/]");
        AnsiConsole.MarkupLine($"[grey]Rate limiter:[/] {(settings.UseRateLimit ? "on" : "off")}   " +
                               $"[grey]rows printed:[/] {(settings.MaxRowsPrinted == 0 ? "all" : settings.MaxRowsPrinted.ToString())}   " +
                               $"[grey]saving:[/] {(settings.SaveResults ? "on" : "off")}");
        AnsiConsole.WriteLine();

        const string settingsEntry = "Settings";
        const string exit = "Exit";
        var entries = tests.Select(t => $"{t.Endpoint}  [grey]{t.Method}[/]").ToList();
        entries.Add(settingsEntry);
        entries.Add(exit);

        var chosen = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Which endpoint do you want to test?")
                .PageSize(24)
                .MoreChoicesText("[grey](move up and down to see more endpoints)[/]")
                .AddChoices(entries));

        if (chosen == exit)
            break;

        if (chosen == settingsEntry)
        {
            var previousRateLimit = settings.UseRateLimit;
            SettingsMenu.Show(settings);
            if (settings.UseRateLimit != previousRateLimit)
            {
                await client.DisposeAsync();
                client = NewClient(settings);
            }

            continue;
        }

        await tests[entries.IndexOf(chosen)].ShowAsync(client, settings, cancellation.Token);
    }
}
finally
{
    await client.DisposeAsync();
}

static OpenF1Client NewClient(AppSettings settings) =>
    new(config: new OpenF1Config { UseRateLimit = settings.UseRateLimit });
