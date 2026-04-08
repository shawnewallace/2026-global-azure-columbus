namespace TaskLibrary.Application.Task;

public interface IDeleteTaskHandler
{
    Task<bool> HandleAsync(Guid id, CancellationToken cancellationToken = default);
}
