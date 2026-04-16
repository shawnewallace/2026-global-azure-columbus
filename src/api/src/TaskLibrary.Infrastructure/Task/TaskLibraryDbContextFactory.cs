using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskLibrary.Infrastructure.Task;

/// <summary>
/// Design-time factory used by EF Core tooling (dotnet ef migrations/update).
/// </summary>
public sealed class TaskLibraryDbContextFactory : IDesignTimeDbContextFactory<TaskLibraryDbContext>
{
    public TaskLibraryDbContext CreateDbContext(string[] args)
    {
        var connectionString = args.Length > 0
            ? args[0]
            : Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
              ?? "Host=db;Port=5432;Database=tasklibrary;Username=tasklibrary;Password=tasklibrary_dev";

        var options = new DbContextOptionsBuilder<TaskLibraryDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new TaskLibraryDbContext(options);
    }
}
