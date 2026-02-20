# ConBot

The terminal-native AI landscape is saturated with complex, stateful "Agentic Workflows" designed for codebase mutation and multi-step reasoning. 

**ConBot is positioned as an anti-agent.** It fills the gap for a low-latency, "search-and-done" reference utility.

## Core Features

* **Stateless Execution:** Operates as a single-shot CLI command. Returns control to the host shell immediately after rendering.
* **AOT Optimized:** Designed for Ahead-of-Time compilation in .NET, ensuring sub-10ms startup times critical for CLI utilities.
* **Dynamic AI Routing:** Implements a Provider Pattern via `Microsoft.Extensions.AI`, allowing seamless switching between OpenAI, OpenRouter, or local API instances without altering core execution logic.
* TBD **OS Context Awareness:** Generates shell-appropriate syntax based on the target environment (PowerShell for Windows, Bash/coreutils for Linux).
* WIP **Styled Output:** Utilizes `Spectre.Console` for isolated, readable terminal UI (TUI) rendering that visually separates answers from standard terminal history.

## Installation

Clone the repository and compile using the .NET CLI. For maximum performance and instant execution, publish as a native AOT binary.

```bash
git clone [https://github.com/yourusername/ConBot.git](https://github.com/yourusername/ConBot.git)
cd ConBot

# For Debian/Linux
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishAot=true

# For Windows 11
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true
```

Move the compiled conbot (or conbot.exe) executable to a directory within your system's $PATH or Environment Variables.

## Configuration

ConBot reads configuration via the Options Pattern. Create an appsettings.json file in the same directory as the compiled executable.

```JSON
{
  "AiSettings": {
    "Provider": "OpenAI",
    "ApiKey": "YOUR_API_KEY_HERE",
    "ModelId": "gpt-4o-mini",
    "EndpointUrl": ""
  }
}
```

* `ApiKey`: Your API token. Keep this file excluded from source control.
* `EndpointUrl`: Leave empty to use the default OpenAI REST endpoint. Populate with an alternative URI (e.g., `https://openrouter.ai/api/v1` or a local Ollama instance) to dynamically reroute the provider.

## Usage

Pass your query as a string argument directly to the executable.

`conbot "How do I extract a .tar.gz file?"`

Output is strictly governed by the system prompt to return only the exact command syntax accompanied by a maximum 3-line explanation.

## Architecture
ConBot is built on C# and .NET. It enforces strict Dependency Inversion, isolating the UI presentation layer (`AppEngine`) from the HTTP REST logic (`IAiProvider`).

* `System.CommandLine` / `args[]`: Intercepts shell arguments for stateless processing.
* `Microsoft.Extensions.DependencyInjection`: Manages the IoC container.
* `Spectre.Console`: Renders ANSI-escaped Markdown panels.