namespace TaskLibrary.Application.Task;

public interface IUpdateTaskHandler
{
    Task<TaskDto?> HandleAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
}
