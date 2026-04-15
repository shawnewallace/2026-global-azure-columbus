using Microsoft.Extensions.Logging;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.Application.Task;

public sealed class ListTasksHandler : IListTasksHandler
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<ListTasksHandler> _logger;

    public ListTasksHandler(ITaskRepository taskRepository, ILogger<ListTasksHandler> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TaskDto>> HandleAsync(string? status, string? priority, string? category, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Listing tasks (status={Status}, priority={Priority}, category={Category})", status, priority, category);
        var tasks = await _taskRepository.FindAllAsync(cancellationToken);
        var filtered = ApplyFilters(tasks, status, priority, category).ToList();
        _logger.LogInformation("Returning {Count} task(s)", filtered.Count);
        return filtered.Select(TaskDto.FromDomain).ToList();
    }

    private static IEnumerable<Domain.Task.Task> ApplyFilters(IReadOnlyList<Domain.Task.Task> tasks, string? status, string? priority, string? category)
    {
        var query = tasks.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<Domain.Task.TaskStatus>(status, ignoreCase: true, out var parsedStatus))
            query = query.Where(t => t.Status == parsedStatus);
        if (!string.IsNullOrWhiteSpace(priority) && Enum.TryParse<Domain.Task.TaskPriority>(priority, ignoreCase: true, out var parsedPriority))
            query = query.Where(t => t.Priority == parsedPriority);
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(t => string.Equals(t.Category, category, StringComparison.OrdinalIgnoreCase));
        return query;
    }
}
