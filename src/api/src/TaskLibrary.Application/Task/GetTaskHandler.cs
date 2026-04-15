using Microsoft.Extensions.Logging;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.Application.Task;

public sealed class GetTaskHandler : IGetTaskHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<GetTaskHandler> _logger;

    public GetTaskHandler(ITaskRepository taskRepository, ILogger<GetTaskHandler> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<TaskDto?> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching task {TaskId}", id);
        var taskId = TaskId.Create(id);
        var task = await _taskRepository.FindByIdAsync(taskId, cancellationToken);
        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} not found", id);
            return null;
        }
        return TaskDto.FromDomain(task);
    }
}
