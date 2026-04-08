namespace TaskLibrary.Application.Task;

public interface ISuggestPriorityHandler
{
    Task<TaskDto?> HandleAsync(Guid id, CancellationToken cancellationToken = default);
}
