using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using TaskLibrary.Application.Task;

namespace TaskLibrary.Api.Functions;

/// <summary>
/// HTTP trigger functions for single-task operations.
/// Routes: GET/PUT/DELETE /api/tasks/{id}
/// </summary>
public sealed class TaskItemFunctions
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly ITaskService _taskService;
    private readonly ILogger<TaskItemFunctions> _logger;

    public TaskItemFunctions(ITaskService taskService, ILogger<TaskItemFunctions> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/tasks/{id}
    /// Returns the task with the given id, or 404 if not found.
    /// </summary>
    [Function(nameof(GetTask))]
    public async Task<HttpResponseData> GetTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tasks/{id:guid}")] HttpRequestData request,
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetTask invoked for {TaskId}", id);

        var task = await _taskService.GetTaskAsync(id, cancellationToken);

        if (task is null)
            return request.CreateResponse(HttpStatusCode.NotFound);

        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(task, JsonOptions), cancellationToken);
        return response;
    }

    /// <summary>
    /// PUT /api/tasks/{id}
    /// Updates the task with the given id.
    /// </summary>
    [Function(nameof(UpdateTask))]
    public async Task<HttpResponseData> UpdateTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "tasks/{id:guid}")] HttpRequestData request,
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateTask invoked for {TaskId}", id);

        var body = await request.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
            return request.CreateResponse(HttpStatusCode.BadRequest);

        UpdateTaskRequest? updateRequest;
        try
        {
            updateRequest = JsonSerializer.Deserialize<UpdateTaskRequest>(body, JsonOptions);
        }
        catch (JsonException)
        {
            return request.CreateResponse(HttpStatusCode.BadRequest);
        }

        if (updateRequest is null)
            return request.CreateResponse(HttpStatusCode.BadRequest);

        var updated = await _taskService.UpdateTaskAsync(id, updateRequest, cancellationToken);

        if (updated is null)
            return request.CreateResponse(HttpStatusCode.NotFound);

        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(updated, JsonOptions), cancellationToken);
        return response;
    }

    /// <summary>
    /// DELETE /api/tasks/{id}
    /// Removes the task with the given id.
    /// </summary>
    [Function(nameof(DeleteTask))]
    public async Task<HttpResponseData> DeleteTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "tasks/{id:guid}")] HttpRequestData request,
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeleteTask invoked for {TaskId}", id);

        var deleted = await _taskService.DeleteTaskAsync(id, cancellationToken);

        return deleted
            ? request.CreateResponse(HttpStatusCode.NoContent)
            : request.CreateResponse(HttpStatusCode.NotFound);
    }
}
