using Microsoft.Extensions.Logging;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.Application.Task;

public sealed class UpdateTaskHandler : IUpdateTaskHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<UpdateTaskHandler> _logger;

    public UpdateTaskHandler(ITaskRepository taskRepository, ILogger<UpdateTaskHandler> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<TaskDto?> HandleAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        _logger.LogInformation("Updating task {TaskId}", id);
        var taskId = TaskId.Create(id);
        var task = await _taskRepository.FindByIdAsync(taskId, cancellationToken);
        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} not found for update", id);
            return null;
        }
        var priority = TaskParser.ParsePriority(request.Priority);
        var status = TaskParser.ParseStatus(request.Status);
        task.UpdateDetails(request.Title, request.Description, priority, request.Category);
        task.ChangeStatus(status);
        if (request.AiSuggestionConfirmed)
            task.ConfirmAiSuggestion();
        await _taskRepository.SaveTaskAsync(task, cancellationToken);
        _logger.LogInformation("Task {TaskId} updated successfully", id);
        return TaskDto.FromDomain(task);
    }
}
