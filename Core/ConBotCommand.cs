using ConBot.Configuration;
using Microsoft.Extensions.Options;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace ConBot.Core;

public enum VerbosityLevel
{
    Short,
    Medium,
    Long
}

// DTO for CLI arguments
public sealed class ConBotSettings : CommandSettings
{
    [CommandArgument(0, "<query>")]
    [Description("The command query to resolve.")]
    public string Query { get; set; } = string.Empty;

    [CommandOption("-v|--verbosity")]
    [Description("Overrides the verbosity setting defined in appsettings.json.")]
    [DefaultValue(null)] // Null falls back to appsettings config
    public VerbosityLevel? Verbosity { get; set; }
}

// Route
public sealed class ConBotCommand(AppEngine engine, IOptions<PromptConfig> promptOptions) : AsyncCommand<ConBotSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ConBotSettings settings, CancellationToken cancellationToken)
    {
        // Mutate the injected configuration if the user passed the CLI flag
        if (settings.Verbosity.HasValue)
        {
            // Convert Enum back to string ("short", "medium", "long") for provider parsing
            promptOptions.Value.Verbosity = settings.Verbosity.Value.ToString().ToLowerInvariant();
        }

        await engine.RunAsync(settings.Query);
        return 0;
    }
}