using TaskLibrary.Domain.Task;

namespace TaskLibrary.Application.Task;

public sealed class UpdateTaskHandler : IUpdateTaskHandler
{
    private readonly ITaskRepository _taskRepository;

    public UpdateTaskHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskDto?> HandleAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var taskId = TaskId.Create(id);
        var task = await _taskRepository.FindByIdAsync(taskId, cancellationToken);
        if (task is null) return null;
        var priority = TaskParser.ParsePriority(request.Priority);
        var status = TaskParser.ParseStatus(request.Status);
        task.UpdateDetails(request.Title, request.Description, priority, request.Category);
        task.ChangeStatus(status);
        await _taskRepository.SaveTaskAsync(task, cancellationToken);
        return TaskDto.FromDomain(task);
    }
}
