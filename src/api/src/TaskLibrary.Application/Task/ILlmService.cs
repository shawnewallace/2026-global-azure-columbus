namespace TaskLibrary.Application.Task;

/// <summary>
/// Abstraction for the LLM integration. Returns AI-generated
/// priority and category suggestions for a given task.
/// </summary>
public interface ILlmService
{
    Task<LlmSuggestion?> SuggestAsync(string title, string? description, CancellationToken cancellationToken = default);
}
