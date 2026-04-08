namespace TaskLibrary.Application.Task;

public interface ICreateTaskHandler
{
    Task<TaskDto> HandleAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
}
