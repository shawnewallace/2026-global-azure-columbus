namespace TaskLibrary.Application.Task;

/// <summary>Request payload for creating a new task.</summary>
public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    string Priority,
    string? Category);
