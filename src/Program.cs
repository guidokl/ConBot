using ConBot.Configuration;
using ConBot.Core;
using ConBot.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System;

Console.OutputEncoding = System.Text.Encoding.UTF8;

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
string activeThemeName = configuration.GetValue<string>("AppSettings:ActiveTheme") ?? "Default";
var themeSection = configuration.GetSection($"Themes:{activeThemeName}");

// Fallback safety check in case of a typo in appsettings.json
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
});

return await app.RunAsync(args);