namespace TaskLibrary.Application.Task;

public interface IListTasksHandler
{
    Task<IReadOnlyList<TaskDto>> HandleAsync(string? status, string? priority, string? category, CancellationToken cancellationToken = default);
}
