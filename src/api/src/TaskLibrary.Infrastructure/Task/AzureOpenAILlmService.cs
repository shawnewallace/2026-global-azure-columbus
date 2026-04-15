using System.Diagnostics;
using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using TaskLibrary.Application.Task;

namespace TaskLibrary.Infrastructure.Task;

/// <summary>
/// Production implementation of <see cref="ILlmService"/> backed by Azure OpenAI.
/// Uses <see cref="DefaultAzureCredential"/> (managed identity) — no key storage required.
/// All exceptions are caught and logged; the method returns <c>null</c> (non-fatal contract).
/// </summary>
public sealed class AzureOpenAILlmService : ILlmService
{
    private const string SystemPrompt =
        "You are a task management assistant. Given a task title and description, respond with JSON: " +
        "{ \"category\": string, \"priority\": \"Low\"|\"Medium\"|\"High\"|\"Critical\", \"reasoning\": string }";

    private readonly ChatClient _chatClient;
    private readonly string _deployment;
    private readonly ILogger<AzureOpenAILlmService> _logger;

    public AzureOpenAILlmService(IConfiguration configuration, ILogger<AzureOpenAILlmService> logger)
    {
        var endpoint = configuration["AZURE_OPENAI_ENDPOINT"]
            ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not configured.");

        _deployment = configuration["AZURE_OPENAI_DEPLOYMENT"] ?? "gpt-4o-mini";
        _logger = logger;

        var client = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential());
        _chatClient = client.GetChatClient(_deployment);
    }

    public async System.Threading.Tasks.Task<LlmSuggestion?> SuggestAsync(
        string title,
        string? description,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var options = new ChatCompletionOptions { ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat() };
            var response = await _chatClient.CompleteChatAsync(BuildMessages(title, description), options, cancellationToken);
            stopwatch.Stop();

            LogCompletion(response.Value.Usage, stopwatch.ElapsedMilliseconds);

            return DeserializeSuggestion(response.Value.Content[0].Text);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "LLM call failed for task title '{Title}' after {LatencyMs}ms", title, stopwatch.ElapsedMilliseconds);
            return null;
        }
    }

    private static List<ChatMessage> BuildMessages(string title, string? description) =>
    [
        new SystemChatMessage(SystemPrompt),
        new UserChatMessage($"Title: {title}\nDescription: {description}")
    ];

    private void LogCompletion(ChatTokenUsage usage, long elapsedMs) =>
        _logger.LogInformation(
            "LLM call completed: model={Deployment}, inputTokens={InputTokens}, outputTokens={OutputTokens}, latencyMs={LatencyMs}",
            _deployment,
            usage.InputTokenCount,
            usage.OutputTokenCount,
            elapsedMs);

    private static LlmSuggestion? DeserializeSuggestion(string json)
    {
        var raw = JsonSerializer.Deserialize<LlmSuggestionResponse>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return raw is null ? null : new LlmSuggestion(raw.Priority, raw.Category, raw.Reasoning);
    }

    private sealed record LlmSuggestionResponse(string Priority, string? Category, string? Reasoning);
}
