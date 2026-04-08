using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using TaskLibrary.Application.Task;

namespace TaskLibrary.Api.Functions;

/// <summary>
/// HTTP trigger functions for listing and creating tasks.
/// Route: GET /api/tasks, POST /api/tasks
/// </summary>
public sealed class TaskCollectionFunctions
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly ITaskService _taskService;
    private readonly ILogger<TaskCollectionFunctions> _logger;

    public TaskCollectionFunctions(ITaskService taskService, ILogger<TaskCollectionFunctions> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/tasks?status=&amp;priority=&amp;category=
    /// Returns all tasks, optionally filtered.
    /// </summary>
    [Function(nameof(ListTasks))]
    public async Task<HttpResponseData> ListTasks(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tasks")] HttpRequestData request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("ListTasks invoked");

        var query = System.Web.HttpUtility.ParseQueryString(request.Url.Query);
        var status = query["status"];
        var priority = query["priority"];
        var category = query["category"];

        var tasks = await _taskService.ListTasksAsync(status, priority, category, cancellationToken);

        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(tasks, JsonOptions), cancellationToken);
        return response;
    }

    /// <summary>
    /// POST /api/tasks
    /// Creates a new task.
    /// </summary>
    [Function(nameof(CreateTask))]
    public async Task<HttpResponseData> CreateTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tasks")] HttpRequestData request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateTask invoked");

        var body = await request.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
            return request.CreateResponse(HttpStatusCode.BadRequest);

        CreateTaskRequest? createRequest;
        try
        {
            createRequest = JsonSerializer.Deserialize<CreateTaskRequest>(body, JsonOptions);
        }
        catch (JsonException)
        {
            return request.CreateResponse(HttpStatusCode.BadRequest);
        }

        if (createRequest is null)
            return request.CreateResponse(HttpStatusCode.BadRequest);

        var created = await _taskService.CreateTaskAsync(createRequest, cancellationToken);

        var response = request.CreateResponse(HttpStatusCode.Created);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(created, JsonOptions), cancellationToken);
        return response;
    }
}
