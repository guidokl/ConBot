# ConBot

The terminal-native AI landscape is saturated with complex, stateful "Agentic Workflows" designed for codebase mutation and multi-step reasoning.

**ConBot is positioned as an anti-agent.** It fills the gap for a low-latency, "search-and-done" reference utility.

## Core Features

* Stateless Execution: Operates as a single-shot CLI command. Returns control to the host shell immediately after rendering.
* Dynamic AI Routing: Implements a Provider Pattern via `Microsoft.Extensions.AI`, allowing seamless switching between OpenAI, OpenRouter, or local API instances without altering core execution logic.
* OS Context Awareness: Generates shell-appropriate syntax based on the target environment configured in settings (e.g., PowerShell for Windows, Bash for Linux).
* Styled TUI Output: Utilizes `Spectre.Console` with a custom state-machine parser to dynamically colorize Markdown outputs. Supports dynamic UI skinning via JSON configuration.
* Deterministic Inference: API constraints (Temperature, Max Tokens) are strictly mapped to deterministic profiles to guarantee reference-only formatting and prevent LLM hallucination.
* Robust CLI Parsing: Leverages `Spectre.Console.Cli` for industry-standard argument validation, strict typing, and auto-generated help screens.

## Installation

Clone the repository and compile using the .NET CLI. ConBot is deployed as a single, self-contained executable bundle (CoreCLR) without requiring the .NET runtime on the host machine.

```bash
git clone https://github.com/guidokl/ConBot.git
cd ConBot

# For Debian/Linux
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true

# For Windows 11
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```

Move the compiled conbot (or conbot.exe) executable to a directory within your system's $PATH or Environment Variables.

## Configuration

ConBot reads configuration via the Options Pattern. Create an appsettings.json file in the same directory as the compiled executable.

```json
{
  "BotSettings": {
    "Provider": "OpenAI",
    "ApiKey": "YOUR_API_KEY_HERE",
    "ModelId": "gpt-4o-mini",
    "EndpointUrl": ""
  },
  "PromptSettings": {
    "OsContext": "Windows 11 PowerShell",
    "Verbosity": "short"
  },
  "ThemeSettings": {
    "PanelBorder": "teal",
    "BlockCode": "lightsalmon1",
    "InlineCode": "blue",
    "Highlight": "yellow",
    "Header": "yellow"
  }
}
```

* ApiKey: Your API token. Keep this file excluded from source control.
* EndpointUrl: Leave empty to use the default OpenAI REST endpoint. Populate with an alternative URI (e.g., https://openrouter.ai/api/v1 or a local Ollama instance) to dynamically reroute the provider.
* ThemeSettings: Accepts standard valid Spectre.Console color names to dynamically skin the UI without recompilation.

## Usage

Pass your query as a string argument directly to the executable.
Bash

```bash
conbot "How do I extract a .tar.gz file?"
```
Output is strictly governed by the system prompt to return only the exact command syntax accompanied by a contextual explanation.
You can dynamically override the configured verbosity setting per invocation utilizing the `-v|--verbosity` flag:
Bash

```bash
conbot -v long "find running containers"
```

Use `conbot --help` to view the auto-generated CLI parameter list and execution options.

## Architecture

ConBot is built on C# and .NET 10. It enforces strict Dependency Inversion, isolating the UI presentation layer (`AppEngine`) from the HTTP REST logic (`IAiProvider`).
- `Spectre.Console.Cli`: Handles strict input validation, `Enum` enforcement, and command routing.
- `Microsoft.Extensions.DependencyInjection`: Bootstraps the Composition Root, bridged to Spectre's internal router via a custom `TypeRegistrar` adapter.
- `AppEngine`: Custom state-machine parser intercepting raw LLM output to parse Markdown blocks and flawlessly render structured UI output via `Spectre.Console`.
