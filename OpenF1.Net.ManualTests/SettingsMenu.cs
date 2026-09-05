using Spectre.Console;

namespace OpenF1.Net.ManualTests;

/// <summary>The global Settings screen — output volume, result saving, and the client's rate limiter.</summary>
public static class SettingsMenu
{
    public static void Show(AppSettings settings)
    {
        const string rows = "Rows printed to the console";
        const string saving = "Save results to file";
        const string directory = "Output folder";
        const string rateLimit = "Client-side rate limiter";
        const string back = "Back";

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]Settings[/]").LeftJustified());

            var choices = new[]
            {
                $"{rows}: {(settings.MaxRowsPrinted == 0 ? "all" : settings.MaxRowsPrinted.ToString())}",
                $"{saving}: {(settings.SaveResults ? "on" : "off")}",
                $"{directory}: {settings.OutputDirectory}",
                $"{rateLimit}: {(settings.UseRateLimit ? "on" : "off")}",
                back,
            };

            var chosen = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("What do you want to change?").AddChoices(choices));

            if (chosen.StartsWith(rows, StringComparison.Ordinal))
            {
                settings.MaxRowsPrinted = AnsiConsole.Prompt(
                    new TextPrompt<int>("How many rows should be printed? [grey](0 = all)[/]")
                        .DefaultValue(settings.MaxRowsPrinted)
                        .Validate(value => value >= 0
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Must be 0 or more[/]")));
            }
            else if (chosen.StartsWith(saving, StringComparison.Ordinal))
            {
                settings.SaveResults = AnsiConsole.Prompt(
                    new SelectionPrompt<string>().Title("Save every result as JSON?").AddChoices("on", "off")) == "on";
            }
            else if (chosen.StartsWith(directory, StringComparison.Ordinal))
            {
                settings.OutputDirectory = AnsiConsole.Prompt(
                    new TextPrompt<string>("Folder to save results into:").DefaultValue(settings.OutputDirectory));
            }
            else if (chosen.StartsWith(rateLimit, StringComparison.Ordinal))
            {
                settings.UseRateLimit = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Pace requests to stay under the API's 3 req/s cap?")
                        .AddChoices("on", "off")) == "on";
            }
            else
            {
                settings.Save();
                return;
            }

            settings.Save();
        }
    }
}
