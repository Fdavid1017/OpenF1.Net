using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenF1.Net.ManualTests;

/// <summary>
/// Global, user-editable knobs for the harness — everything reachable from the Settings menu. Persisted
/// next to the executable so a run's setup survives into the next one.
/// </summary>
public class AppSettings
{
    static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "settings.json");

    static readonly JsonSerializerOptions FileOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    };

    /// <summary>How many result rows to print to the console. 0 means "all of them".</summary>
    public int MaxRowsPrinted { get; set; } = 10;

    /// <summary>When true, every run also writes its full JSON result into <see cref="OutputDirectory"/>.</summary>
    public bool SaveResults { get; set; }

    /// <summary>Folder the full JSON results are written to when <see cref="SaveResults"/> is on.</summary>
    public string OutputDirectory { get; set; } = Path.Combine(AppContext.BaseDirectory, "results");

    /// <summary>Mirrors <see cref="OpenF1Config.UseRateLimit"/> — the client is rebuilt when this changes.</summary>
    public bool UseRateLimit { get; set; } = true;

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(FilePath))
                return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath), FileOptions) ?? new AppSettings();
        }
        catch (Exception)
        {
            // A corrupt settings file is never worth failing the harness over — fall back to defaults.
        }

        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this, FileOptions));
        }
        catch (Exception)
        {
            // Same here: persistence is a convenience, not a requirement.
        }
    }
}
