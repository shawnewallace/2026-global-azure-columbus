using Microsoft.EntityFrameworkCore;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.Infrastructure.Task;

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
        return records.Select(TaskEntityMapper.ToDomain).ToList();
    }

    public async Task<Domain.Task.Task?> FindByIdAsync(TaskId id, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Tasks.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id.Value, cancellationToken);
        return record is null ? null : TaskEntityMapper.ToDomain(record);
    }

    public async System.Threading.Tasks.Task SaveNewTaskAsync(Domain.Task.Task task, CancellationToken cancellationToken = default)
    {
        var record = TaskEntityMapper.ToRecord(task);
        _dbContext.Tasks.Add(record);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task SaveTaskAsync(Domain.Task.Task task, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Tasks.FindAsync([task.Id.Value], cancellationToken);
        if (record is null) return;
        TaskEntityMapper.UpdateRecord(task, record);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(TaskId id, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Tasks.FindAsync([id.Value], cancellationToken);
        if (record is null) return;
        _dbContext.Tasks.Remove(record);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
