using ConBot.Configuration;
using ConBot.Providers;
using Microsoft.Extensions.Options;
using Spectre.Console;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ConBot.Core;

public class AppEngine(IAiProvider aiProvider, IOptions<ThemeConfig> themeOptions)
{
    private readonly ThemeConfig _theme = themeOptions.Value;

    // Regex Definitions
#pragma warning disable SYSLIB1045
    private static readonly Regex InlineCodeRegex = new(@"`([^`]+)`", RegexOptions.Compiled);
    private static readonly Regex BoldRegex = new(@"\*\*([^*]+)\*\*", RegexOptions.Compiled);
    private static readonly Regex ItalicRegex = new(@"(?<!\*)\*([^*]+)\*(?!\*)", RegexOptions.Compiled);
#pragma warning restore SYSLIB1045

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
                .Header($"[{_theme.PanelBorder}] [/]" +
                        $"c[{_theme.BlockCode}]●[/]nb[{_theme.InlineCode}]●[/]t" +
                        $"[{_theme.PanelBorder}] [/]", Justify.Left)
                .Expand()
                .Padding(1, 1, 1, 1)
                .BorderStyle(Style.Parse(_theme.PanelBorder));
            AnsiConsole.Write(panel);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error executing request: {ex.Message}[/]");
        }
    }

    private string ApplyInlineFormatting(string escapedLine)
    {
        // Apply inline style regexes
        var formatted = InlineCodeRegex.Replace(escapedLine, $"[{_theme.InlineCode}]$1[/]");
        formatted = BoldRegex.Replace(formatted, $"[bold {_theme.Highlight}]$1[/]");
        formatted = ItalicRegex.Replace(formatted, $"[italic {_theme.Highlight}]$1[/]");
        return formatted;
    }

    private string ToSpectreMarkup(string response)
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
                    sb.AppendLine();
                inCodeBlock = !inCodeBlock;
                continue;
            }

            if (inCodeBlock)
            {
                // We are inside the code block. 
                sb.AppendLine($"> [{_theme.BlockCode}]{escaped}[/]");
            }
            else if (escaped.StartsWith("### ") || escaped.StartsWith("## ") || escaped.StartsWith("# "))
            {
                sb.AppendLine($"[bold {_theme.Header}]{ApplyInlineFormatting(escaped)}[/]");
            }
            else
            {
                sb.AppendLine(ApplyInlineFormatting(escaped));
            }
        }

        return sb.ToString().TrimEnd();
    }
}