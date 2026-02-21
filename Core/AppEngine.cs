using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ConBot.Providers;
using Spectre.Console;

namespace ConBot.Core;

public class AppEngine(IAiProvider aiProvider)
{
    // Regex targets text between backticks
    private static readonly Regex InlineCode = new(@"`([^`]+)`", RegexOptions.Compiled);

    public async Task RunAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            AnsiConsole.MarkupLine("[red]Error: Query cannot be empty.[/]");
            return;
        }

        try
        {
            var response = await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync("Thinking...", async _ => await aiProvider.GetCommandAsync(query));

            var panel = new Panel(new Markup(ToSpectreMarkup(response)))
                .Border(BoxBorder.Rounded)
                .Header("[teal] [/]c[blue]●[/]nb[LightSalmon1]●[/]t[teal] [/]", Justify.Left)
                .Expand()
                .Padding(1, 1, 1, 1)
                .BorderStyle(Style.Parse("Teal"));

            AnsiConsole.Write(panel);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error executing request: {ex.Message}[/]");
        }
    }

    private static string ColorInlineCode(string escapedLine)
    {
        return InlineCode.Replace(escapedLine, "[blue]$1[/]");
    }

    private static string ToSpectreMarkup(string response)
    {
        var sb = new StringBuilder();
        var inCodeBlock = false;

        // StringReader prevents allocating a string[] for the entire response
        using var reader = new StringReader(response);
        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            var escaped = line.EscapeMarkup();

            if (escaped.StartsWith("```"))
            {
                if (inCodeBlock)
                {
                    // We are exiting the code block. 
                    sb.AppendLine();
                }

                inCodeBlock = !inCodeBlock;
                continue;
            }

            if (inCodeBlock)
            {
                // We are inside the code block. 
                sb.AppendLine($"> [LightSalmon1]{escaped}[/]");
            }
            else if (escaped.StartsWith("### ") || escaped.StartsWith("## ") || escaped.StartsWith("# "))
            {
                sb.AppendLine($"[bold yellow]{ColorInlineCode(escaped)}[/]");
            }
            else
            {
                sb.AppendLine(ColorInlineCode(escaped));
            }
        }

        return sb.ToString().TrimEnd();
    }
}