using Microsoft.EntityFrameworkCore;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.Infrastructure.Task;

/// <summary>
/// EF Core implementation of <see cref="ITaskRepository"/>.
/// Translates between <see cref="TaskRecord"/> (persistence) and
/// <see cref="Domain.Task.Task"/> (domain aggregate).
/// </summary>
public sealed class TaskRepository : ITaskRepository
{
    private readonly TaskLibraryDbContext _dbContext;

    public TaskRepository(TaskLibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Domain.Task.Task>> FindAllAsync(CancellationToken cancellationToken = default)
    {
        var records = await _dbContext.Tasks.AsNoTracking().ToListAsync(cancellationToken);
        return records.Select(ToDomain).ToList();
    }

    public async Task<Domain.Task.Task?> FindByIdAsync(TaskId id, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Tasks.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id.Value, cancellationToken);

        return record is null ? null : ToDomain(record);
    }

    public async System.Threading.Tasks.Task AddAsync(Domain.Task.Task task, CancellationToken cancellationToken = default)
    {
        var record = ToRecord(task);
        _dbContext.Tasks.Add(record);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task UpdateAsync(Domain.Task.Task task, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Tasks.FindAsync([task.Id.Value], cancellationToken);
        if (record is null) return;

        MapToRecord(task, record);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task RemoveAsync(TaskId id, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Tasks.FindAsync([id.Value], cancellationToken);
        if (record is null) return;

        _dbContext.Tasks.Remove(record);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static Domain.Task.Task ToDomain(TaskRecord record)
    {
        var taskId = TaskId.Create(record.Id);
        var status = Enum.Parse<Domain.Task.TaskStatus>(record.Status);
        var priority = Enum.Parse<Domain.Task.TaskPriority>(record.Priority);
        Domain.Task.TaskPriority? aiPriority = record.AiSuggestedPriority is not null
            ? Enum.Parse<Domain.Task.TaskPriority>(record.AiSuggestedPriority)
            : null;

        return Domain.Task.Task.Reconstitute(
            taskId,
            record.Title,
            record.Description,
            status,
            priority,
            record.Category,
            aiPriority,
            record.AiSuggestedCategory,
            record.AiReasoning,
            record.AiSuggestionConfirmed,
            record.CreatedAt,
            record.UpdatedAt);
    }

    private static TaskRecord ToRecord(Domain.Task.Task task) =>
        new()
        {
            Id = task.Id.Value,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            Category = task.Category,
            AiSuggestedPriority = task.AiSuggestedPriority?.ToString(),
            AiSuggestedCategory = task.AiSuggestedCategory,
            AiReasoning = task.AiReasoning,
            AiSuggestionConfirmed = task.AiSuggestionConfirmed,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

    private static void MapToRecord(Domain.Task.Task task, TaskRecord record)
    {
        record.Title = task.Title;
        record.Description = task.Description;
        record.Status = task.Status.ToString();
        record.Priority = task.Priority.ToString();
        record.Category = task.Category;
        record.AiSuggestedPriority = task.AiSuggestedPriority?.ToString();
        record.AiSuggestedCategory = task.AiSuggestedCategory;
        record.AiReasoning = task.AiReasoning;
        record.AiSuggestionConfirmed = task.AiSuggestionConfirmed;
        record.UpdatedAt = task.UpdatedAt;
    }
}
