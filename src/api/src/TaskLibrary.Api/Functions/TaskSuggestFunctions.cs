using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using TaskLibrary.Application.Task;

namespace TaskLibrary.Api.Functions;

/// <summary>
/// HTTP trigger function for triggering an AI suggestion for a task.
/// Route: POST /api/tasks/{id}/suggest
/// </summary>
public sealed class TaskSuggestFunctions
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskSuggestFunctions> _logger;

    public TaskSuggestFunctions(ITaskService taskService, ILogger<TaskSuggestFunctions> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>
    /// POST /api/tasks/{id}/suggest
    /// Triggers the LLM suggestion pipeline for the given task.
    /// </summary>
    [Function(nameof(SuggestTaskPriority))]
    public async Task<HttpResponseData> SuggestTaskPriority(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tasks/{id:guid}/suggest")] HttpRequestData request,
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("SuggestTaskPriority invoked for {TaskId}", id);

        var updated = await _taskService.SuggestPriorityAsync(id, cancellationToken);

        if (updated is null)
            return request.CreateResponse(HttpStatusCode.NotFound);

        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(System.Text.Json.JsonSerializer.Serialize(updated), cancellationToken);
        return response;
    }
}
