using System;
using System.Collections.Generic;
using System.Text;
using ConBot.Providers;
using Spectre.Console;

namespace ConBot.Core;

public class AppEngine(IAiProvider aiProvider)
{
    public async Task RunAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            AnsiConsole.MarkupLine("[red]Error: Query cannot be empty.[/]");
            return;
        }

        try
        {
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync("Thinking...", async ctx =>
                {
                    var response = await aiProvider.GetCommandAsync(query);

                    var panel = new Panel(new Text(response))
                        .Header("Result")
                        .BorderColor(Color.Blue)
                        .Padding(1, 1, 1, 1);

                    AnsiConsole.Write(panel);
                });
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error executing request: {ex.Message}[/]");
        }
    }
}