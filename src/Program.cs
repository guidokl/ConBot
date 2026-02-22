using ConBot.Configuration;
using ConBot.Core;
using ConBot.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// --- PRE-PROCESSOR LOGIC ---
// Allows users to input unquoted queries with multiple words
var processedArgs = new List<string>();
var queryParts = new List<string>();
string? themeOverride = null;

for (int i = 0; i < args.Length; i++)
{
    if (args[i].StartsWith("-"))
    {
        // It's a flag (like -v or --help), keep it
        processedArgs.Add(args[i]);

        // If the flag expects a value (-v <level>), capture the next arg too 
        // so it doesn't accidentally get stitched into the query
        if ((args[i] == "-v" || args[i] == "--verbosity") && i + 1 < args.Length)
        {
            processedArgs.Add(args[i + 1]);
            i++;
        }
        else if ((args[i] == "-t" || args[i] == "--theme") && i + 1 < args.Length)
        {
            processedArgs.Add(args[i + 1]);
            themeOverride = args[i + 1]; // Capture the theme override
            i++;
        }
    }
    else
    {
        // It's a regular word, queue it up
        queryParts.Add(args[i]);
    }
}

// Stitch all unquoted words into a single, cohesive string
if (queryParts.Count > 0)
{
    processedArgs.Add(string.Join(" ", queryParts));
}


var exePath = AppContext.BaseDirectory;

var configuration = new ConfigurationBuilder()
    .SetBasePath(exePath)
    .AddJsonFile("appthemes.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();
services.Configure<BotConfig>(configuration.GetSection(BotConfig.SectionName));
services.Configure<PromptConfig>(configuration.GetSection(PromptConfig.SectionName));

// Extract the active theme name from AppSettings:ActiveTheme, defaulting to "Default"
// OVERRIDE: Check the CLI flag first
string activeThemeName = themeOverride ?? configuration.GetValue<string>("AppSettings:ActiveTheme") ?? "Default";
var themeSection = configuration.GetSection($"Themes:{activeThemeName}");

// Fallback safety check in case of a typo in appsettings.json or CLI flag
if (!themeSection.Exists())
{
    themeSection = configuration.GetSection("Themes:Default");
    Console.WriteLine($"[Warning] Theme '{activeThemeName}' not found in appthemes.json. Falling back to Default.");
}

// Bind the dynamically resolved JSON node to the ThemeConfig POCO
services.Configure<ThemeConfig>(themeSection);

services.AddTransient<IAiProvider, OpenAiProvider>();
services.AddTransient<AppEngine>();

// Establish Spectre.Console.Cli routing mapping to Microsoft DI
var registrar = new TypeRegistrar(services);
var app = new CommandApp<ConBotCommand>(registrar);

// Build automated --help screen and validation rules
app.Configure(config =>
{
    config.SetApplicationName("conbot");
    config.AddExample(["\"extract tar gz\""]);
    config.AddExample(["\"find running containers\"", "-v", "medium"]);
    config.AddExample(["list docker networks", "-t", "Cyberpunk"]);
});

return await app.RunAsync(processedArgs.ToArray());