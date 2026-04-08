namespace TaskLibrary.Domain.Task;

public interface ITaskRepository
{
    Task<IReadOnlyList<Task>> FindAllAsync(CancellationToken cancellationToken = default);
    Task<Task?> FindByIdAsync(TaskId id, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task SaveNewTaskAsync(Task task, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task SaveTaskAsync(Task task, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task DeleteTaskAsync(TaskId id, CancellationToken cancellationToken = default);
}
