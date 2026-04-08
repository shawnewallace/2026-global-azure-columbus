namespace TaskLibrary.Application.Task;

/// <summary>
/// Represents an AI-generated suggestion for a task's priority and category.
/// </summary>
public sealed record LlmSuggestion(
    string Priority,
    string? Category,
    string? Reasoning);
