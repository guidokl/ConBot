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
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();
services.Configure<BotConfig>(configuration.GetSection(BotConfig.SectionName));
services.Configure<PromptConfig>(configuration.GetSection(PromptConfig.SectionName));
services.Configure<ThemeConfig>(configuration.GetSection(ThemeConfig.SectionName));

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