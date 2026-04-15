using Microsoft.Extensions.Configuration;
using TaskLibrary.Application.Task;
using TaskLibrary.Domain.Task;
using TaskLibrary.Infrastructure.Task;

namespace TaskLibrary.Api.Task;

public static class TaskServiceRegistration
{
    public static IServiceCollection AddTaskServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ITaskRepository, TaskRepository>();

        var endpoint = config["AZURE_OPENAI_ENDPOINT"];
        if (!string.IsNullOrEmpty(endpoint))
            services.AddScoped<ILlmService, AzureOpenAILlmService>();
        else
            services.AddScoped<ILlmService, LlmServiceStub>();

        services.AddScoped<ICreateTaskHandler, CreateTaskHandler>();
        services.AddScoped<IGetTaskHandler, GetTaskHandler>();
        services.AddScoped<IListTasksHandler, ListTasksHandler>();
        services.AddScoped<IUpdateTaskHandler, UpdateTaskHandler>();
        services.AddScoped<IDeleteTaskHandler, DeleteTaskHandler>();
        services.AddScoped<ISuggestPriorityHandler, SuggestPriorityHandler>();

        return services;
    }
}
