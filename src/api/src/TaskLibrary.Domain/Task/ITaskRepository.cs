namespace TaskLibrary.Domain.Task;

/// <summary>
/// Repository interface for the Task aggregate root.
/// Defined in the Domain layer; implemented in Infrastructure.
/// </summary>
public interface ITaskRepository
{
    Task<IReadOnlyList<Task>> FindAllAsync(CancellationToken cancellationToken = default);

    Task<Task?> FindByIdAsync(TaskId id, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task AddAsync(Task task, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task UpdateAsync(Task task, CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task RemoveAsync(TaskId id, CancellationToken cancellationToken = default);
}
