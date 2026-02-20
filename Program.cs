using ConBot.Configuration;
using ConBot.Core;
using ConBot.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// 1. Argument Parsing
if (args.Length == 0)
{
    Console.WriteLine("Usage: conbot \"<your command query>\"");
    return;
}

// Join handles both quoted (conbot "list files") and unquoted (conbot list files) inputs
string query = string.Join(" ", args);

// 2. Configuration Binding
// Resolve the absolute path of the executing binary to locate appsettings.json
var exePath = AppContext.BaseDirectory;

var configuration = new ConfigurationBuilder()
    .SetBasePath(exePath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// 3. Dependency Injection Setup
var services = new ServiceCollection();
services.Configure<BotConfig>(configuration.GetSection(BotConfig.SectionName));
services.AddTransient<IAiProvider, OpenAiProvider>();
services.AddTransient<AppEngine>();

var serviceProvider = services.BuildServiceProvider();

// 4. Execution
var engine = serviceProvider.GetRequiredService<AppEngine>();
await engine.RunAsync(query);