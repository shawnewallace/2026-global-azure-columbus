namespace TaskLibrary.Application.Task;

/// <summary>Request payload for updating an existing task.</summary>
public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    string Priority,
    string? Category,
    string Status,
    bool AiSuggestionConfirmed = false);
