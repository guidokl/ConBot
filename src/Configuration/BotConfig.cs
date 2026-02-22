using System;
using System.Collections.Generic;
using System.Text;

namespace ConBot.Configuration
{
    public class BotConfig
    {
        // Constant used to target the specific JSON node during dependency injection
        public const string SectionName = "BotSettings";

        public string Provider { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ModelId { get; set; } = string.Empty;

        // Optional: Used if routing to OpenRouter or an Ollama local instance
        public string? EndpointUrl { get; set; }
    }
}
