using Microsoft.Extensions.Logging;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.Application.Task;

public sealed class SuggestPriorityHandler : ISuggestPriorityHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILlmService _llmService;
    private readonly ILogger<SuggestPriorityHandler> _logger;

    public SuggestPriorityHandler(ITaskRepository taskRepository, ILlmService llmService, ILogger<SuggestPriorityHandler> logger)
    {
        _taskRepository = taskRepository;
        _llmService = llmService;
        _logger = logger;
    }

    public async Task<TaskDto?> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Requesting AI priority suggestion for task {TaskId}", id);
        var taskId = TaskId.Create(id);
        var task = await _taskRepository.FindByIdAsync(taskId, cancellationToken);
        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} not found for suggestion", id);
            return null;
        }
        var suggestion = await _llmService.SuggestAsync(task.Title, task.Description, cancellationToken);
        if (suggestion is null)
        {
            _logger.LogWarning("LLM returned no suggestion for task {TaskId}", id);
            return TaskDto.FromDomain(task);
        }
        var suggestedPriority = TaskParser.ParsePriority(suggestion.Priority);
        task.ApplyAiSuggestion(suggestedPriority, suggestion.Category, suggestion.Reasoning);
        await _taskRepository.SaveTaskAsync(task, cancellationToken);
        _logger.LogInformation("AI suggestion applied to task {TaskId}: priority={Priority}", id, suggestion.Priority);
        return TaskDto.FromDomain(task);
    }
}
