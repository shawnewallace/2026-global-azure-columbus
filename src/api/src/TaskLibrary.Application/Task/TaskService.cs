using TaskLibrary.Domain.Task;

namespace TaskLibrary.Application.Task;

/// <summary>
/// Orchestrates all task use cases, delegating persistence to
/// <see cref="ITaskRepository"/> and AI suggestions to <see cref="ILlmService"/>.
/// </summary>
public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILlmService _llmService;

    public TaskService(ITaskRepository taskRepository, ILlmService llmService)
    {
        _taskRepository = taskRepository;
        _llmService = llmService;
    }

    public async Task<IReadOnlyList<TaskDto>> ListTasksAsync(
        string? status,
        string? priority,
        string? category,
        CancellationToken cancellationToken = default)
    {
        var tasks = await _taskRepository.FindAllAsync(cancellationToken);
        var filtered = ApplyFilters(tasks, status, priority, category);
        return filtered.Select(TaskDto.FromDomain).ToList();
    }

    public async Task<TaskDto?> GetTaskAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var taskId = TaskId.Create(id);
        var task = await _taskRepository.FindByIdAsync(taskId, cancellationToken);
        return task is null ? null : TaskDto.FromDomain(task);
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        var priority = ParsePriority(request.Priority);
        var task = Domain.Task.Task.Create(request.Title, request.Description, priority, request.Category);

        await _taskRepository.AddAsync(task, cancellationToken);
        return TaskDto.FromDomain(task);
    }

    public async Task<TaskDto?> UpdateTaskAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        var taskId = TaskId.Create(id);
        var task = await _taskRepository.FindByIdAsync(taskId, cancellationToken);
        if (task is null) return null;

        var priority = ParsePriority(request.Priority);
        var status = ParseStatus(request.Status);

        task.UpdateDetails(request.Title, request.Description, priority, request.Category);
        task.ChangeStatus(status);

        await _taskRepository.UpdateAsync(task, cancellationToken);
        return TaskDto.FromDomain(task);
    }

    public async Task<bool> DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var taskId = TaskId.Create(id);
        var task = await _taskRepository.FindByIdAsync(taskId, cancellationToken);
        if (task is null) return false;

        await _taskRepository.RemoveAsync(taskId, cancellationToken);
        return true;
    }

    public async Task<TaskDto?> SuggestPriorityAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var taskId = TaskId.Create(id);
        var task = await _taskRepository.FindByIdAsync(taskId, cancellationToken);
        if (task is null) return null;

        var suggestion = await _llmService.SuggestAsync(task.Title, task.Description, cancellationToken);
        if (suggestion is null) return TaskDto.FromDomain(task);

        var suggestedPriority = ParsePriority(suggestion.Priority);
        task.ApplyAiSuggestion(suggestedPriority, suggestion.Category, suggestion.Reasoning);

        await _taskRepository.UpdateAsync(task, cancellationToken);
        return TaskDto.FromDomain(task);
    }

    private static IEnumerable<Domain.Task.Task> ApplyFilters(
        IReadOnlyList<Domain.Task.Task> tasks,
        string? status,
        string? priority,
        string? category)
    {
        var query = tasks.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<Domain.Task.TaskStatus>(status, ignoreCase: true, out var parsedStatus))
            query = query.Where(t => t.Status == parsedStatus);

        if (!string.IsNullOrWhiteSpace(priority) && Enum.TryParse<TaskPriority>(priority, ignoreCase: true, out var parsedPriority))
            query = query.Where(t => t.Priority == parsedPriority);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(t => string.Equals(t.Category, category, StringComparison.OrdinalIgnoreCase));

        return query;
    }

    private static TaskPriority ParsePriority(string priority)
    {
        if (!Enum.TryParse<TaskPriority>(priority, ignoreCase: true, out var parsed))
            throw new ArgumentException($"Invalid priority value: '{priority}'.", nameof(priority));
        return parsed;
    }

    private static Domain.Task.TaskStatus ParseStatus(string status)
    {
        if (!Enum.TryParse<Domain.Task.TaskStatus>(status, ignoreCase: true, out var parsed))
            throw new ArgumentException($"Invalid status value: '{status}'.", nameof(status));
        return parsed;
    }
}
