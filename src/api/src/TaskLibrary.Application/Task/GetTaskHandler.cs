using TaskLibrary.Domain.Task;

namespace TaskLibrary.Application.Task;

public sealed class GetTaskHandler : IGetTaskHandler
{
    private readonly ITaskRepository _taskRepository;

    public GetTaskHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskDto?> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var taskId = TaskId.Create(id);
        var task = await _taskRepository.FindByIdAsync(taskId, cancellationToken);
        return task is null ? null : TaskDto.FromDomain(task);
    }
}
