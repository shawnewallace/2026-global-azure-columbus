using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskLibrary.Application.Task;
using TaskLibrary.Domain.Task;
using TaskLibrary.Infrastructure.Task;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration["ConnectionStrings__DefaultConnection"]
            ?? "Host=localhost;Database=tasklibrary;Username=postgres;Password=postgres";

        services.AddDbContext<TaskLibraryDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ILlmService, LlmServiceStub>();
        services.AddScoped<ITaskService, TaskService>();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

await host.RunAsync();
