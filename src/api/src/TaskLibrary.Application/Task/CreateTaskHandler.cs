using TaskLibrary.Domain.Task;

namespace TaskLibrary.Application.Task;

public sealed class CreateTaskHandler : ICreateTaskHandler
{
    private readonly ITaskRepository _taskRepository;

    public CreateTaskHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskDto> HandleAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var priority = TaskParser.ParsePriority(request.Priority);
        var task = Domain.Task.Task.Create(request.Title, request.Description, priority, request.Category);
        await _taskRepository.SaveNewTaskAsync(task, cancellationToken);
        return TaskDto.FromDomain(task);
    }
}
