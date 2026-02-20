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
    private readonly PromptConfig _promptConfig;

    // Constructor parameters suffixed with 'Options'
    public OpenAiProvider(IOptions<BotConfig> botOptions, IOptions<PromptConfig> promptOptions)
    {
        // Local and class-level fields retain the full descriptive names
        var botConfig = botOptions.Value;
        _promptConfig = promptOptions.Value;

        var clientOptions = new OpenAIClientOptions();

        if (!string.IsNullOrWhiteSpace(botConfig.EndpointUrl))
        {
            clientOptions.Endpoint = new Uri(botConfig.EndpointUrl);
        }

        var openAiClient = new OpenAIClient(new ApiKeyCredential(botConfig.ApiKey), clientOptions);
        var nativeChatClient = openAiClient.GetChatClient(botConfig.ModelId);

        _chatClient = nativeChatClient.AsIChatClient();
    }

    public async Task<string> GetCommandAsync(string prompt, CancellationToken cancellationToken = default)
    {
        IList<ChatMessage> messages =
        [
            new ChatMessage(ChatRole.System, SystemPrompts.GetSystemBehavior(_promptConfig.OsContext, _promptConfig.Verbosity)),
            new ChatMessage(ChatRole.User, prompt)
        ];

        var response = await _chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);

        return response.Text ?? "Error: Empty response returned from provider.";
    }
}