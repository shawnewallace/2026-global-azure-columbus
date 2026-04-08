using Microsoft.EntityFrameworkCore;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.Infrastructure.Task;

/// <summary>
/// EF Core DbContext for the Task Library.
/// Uses the PostgreSQL provider via Npgsql.
/// </summary>
public sealed class TaskLibraryDbContext : DbContext
{
    public TaskLibraryDbContext(DbContextOptions<TaskLibraryDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskRecord> Tasks => Set<TaskRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskLibraryDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
