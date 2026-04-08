using TaskLibrary.Domain.Task;

namespace TaskLibrary.Application.Task;

public sealed class DeleteTaskHandler : IDeleteTaskHandler
{
    private readonly ITaskRepository _taskRepository;

    public DeleteTaskHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<bool> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var taskId = TaskId.Create(id);
        var task = await _taskRepository.FindByIdAsync(taskId, cancellationToken);
        if (task is null) return false;
        await _taskRepository.DeleteTaskAsync(taskId, cancellationToken);
        return true;
    }
}
