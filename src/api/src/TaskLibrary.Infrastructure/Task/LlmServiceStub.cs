using TaskLibrary.Application.Task;

namespace TaskLibrary.Infrastructure.Task;

/// <summary>
/// Stub implementation of <see cref="ILlmService"/>.
/// Returns a placeholder suggestion. Replace with a real LLM client for production.
/// </summary>
public sealed class LlmServiceStub : ILlmService
{
    public System.Threading.Tasks.Task<LlmSuggestion?> SuggestAsync(
        string title,
        string? description,
        CancellationToken cancellationToken = default)
    {
        // Stub: returns null to signal no suggestion available.
        // A real implementation would call an Azure OpenAI / OpenAI endpoint here.
        LlmSuggestion? suggestion = null;
        return System.Threading.Tasks.Task.FromResult(suggestion);
    }
}
