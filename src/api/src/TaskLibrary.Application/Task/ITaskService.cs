namespace TaskLibrary.Application.Task;

/// <summary>
/// Application service interface for all task use cases.
/// Callers (e.g., the API layer) depend on this abstraction.
/// </summary>
public interface ITaskService
{
    Task<IReadOnlyList<TaskDto>> ListTasksAsync(
        string? status,
        string? priority,
        string? category,
        CancellationToken cancellationToken = default);

    Task<TaskDto?> GetTaskAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);

    Task<TaskDto?> UpdateTaskAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TaskDto?> SuggestPriorityAsync(Guid id, CancellationToken cancellationToken = default);
}
