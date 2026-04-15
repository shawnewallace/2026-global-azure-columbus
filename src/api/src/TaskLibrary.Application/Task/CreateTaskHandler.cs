using Microsoft.Extensions.Logging;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.Application.Task;

public sealed class CreateTaskHandler : ICreateTaskHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<CreateTaskHandler> _logger;

    public CreateTaskHandler(ITaskRepository taskRepository, ILogger<CreateTaskHandler> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<TaskDto> HandleAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        _logger.LogInformation("Creating task with title '{Title}'", request.Title);
        var priority = TaskParser.ParsePriority(request.Priority);
        var task = Domain.Task.Task.Create(request.Title, request.Description, priority, request.Category);
        await _taskRepository.SaveNewTaskAsync(task, cancellationToken);
        _logger.LogInformation("Task created. TaskId={TaskId} Title={Title}", task.Id.Value, task.Title);
        return TaskDto.FromDomain(task);
    }
}
