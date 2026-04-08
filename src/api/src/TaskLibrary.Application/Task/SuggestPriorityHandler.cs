using TaskLibrary.Domain.Task;

namespace TaskLibrary.Application.Task;

public sealed class SuggestPriorityHandler : ISuggestPriorityHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILlmService _llmService;

    public SuggestPriorityHandler(ITaskRepository taskRepository, ILlmService llmService)
    {
        _taskRepository = taskRepository;
        _llmService = llmService;
    }

    public async Task<TaskDto?> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var taskId = TaskId.Create(id);
        var task = await _taskRepository.FindByIdAsync(taskId, cancellationToken);
        if (task is null) return null;
        var suggestion = await _llmService.SuggestAsync(task.Title, task.Description, cancellationToken);
        if (suggestion is null) return TaskDto.FromDomain(task);
        var suggestedPriority = TaskParser.ParsePriority(suggestion.Priority);
        task.ApplyAiSuggestion(suggestedPriority, suggestion.Category, suggestion.Reasoning);
        await _taskRepository.SaveTaskAsync(task, cancellationToken);
        return TaskDto.FromDomain(task);
    }
}
