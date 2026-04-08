namespace TaskLibrary.Application.Task;

public interface IGetTaskHandler
{
    Task<TaskDto?> HandleAsync(Guid id, CancellationToken cancellationToken = default);
}
