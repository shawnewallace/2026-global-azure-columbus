using TaskLibrary.Domain.Task;

namespace TaskLibrary.Application.Task;

/// <summary>Represents a task as exposed to callers outside the domain.</summary>
public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    string Priority,
    string? Category,
    string? AiSuggestedPriority,
    string? AiSuggestedCategory,
    string? AiReasoning,
    bool AiSuggestionConfirmed,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    /// <summary>Maps a domain Task aggregate to a TaskDto.</summary>
    public static TaskDto FromDomain(Domain.Task.Task task) =>
        new(
            task.Id.Value,
            task.Title,
            task.Description,
            task.Status.ToString(),
            task.Priority.ToString(),
            task.Category,
            task.AiSuggestedPriority?.ToString(),
            task.AiSuggestedCategory,
            task.AiReasoning,
            task.AiSuggestionConfirmed,
            task.CreatedAt,
            task.UpdatedAt);
}
