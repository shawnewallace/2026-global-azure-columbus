using Microsoft.Extensions.Logging;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.Application.Task;

public sealed class DeleteTaskHandler : IDeleteTaskHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<DeleteTaskHandler> _logger;

    public DeleteTaskHandler(ITaskRepository taskRepository, ILogger<DeleteTaskHandler> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting task {TaskId}", id);
        var taskId = TaskId.Create(id);
        var task = await _taskRepository.FindByIdAsync(taskId, cancellationToken);
        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} not found for deletion", id);
            return false;
        }
        await _taskRepository.DeleteTaskAsync(taskId, cancellationToken);
        _logger.LogInformation("Task {TaskId} deleted successfully", id);
        return true;
    }
}
