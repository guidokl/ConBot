using ConBot.Configuration;
using ConBot.Core;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using System.ClientModel;
using OpenAI;

namespace ConBot.Providers;

public class OpenAiProvider : IAiProvider
{
    private readonly IChatClient _chatClient;

    public OpenAiProvider(IOptions<BotConfig> config)
    {
        var botConfig = config.Value;

        var options = new OpenAIClientOptions();

        if (!string.IsNullOrWhiteSpace(botConfig.EndpointUrl))
        {
            options.Endpoint = new Uri(botConfig.EndpointUrl);
        }

        // Initialize the top-level client, then extract the ChatClient
        var openAiClient = new OpenAIClient(new ApiKeyCredential(botConfig.ApiKey), options);
        var nativeChatClient = openAiClient.GetChatClient(botConfig.ModelId);

        _chatClient = nativeChatClient.AsIChatClient();
    }

    public async Task<string> GetCommandAsync(string prompt, CancellationToken cancellationToken = default)
    {
        IList<ChatMessage> messages =
        [
            new ChatMessage(ChatRole.System, SystemPrompts.SystemBehavior),
            new ChatMessage(ChatRole.User, prompt)
        ];

        var response = await _chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);

        return response.Text ?? "Error: Empty response returned from provider.";
    }
}