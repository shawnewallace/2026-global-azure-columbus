using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using TaskLibrary.Application.Task;

namespace TaskLibrary.Api.Functions;

public sealed class TaskSuggestFunctions
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly ISuggestPriorityHandler _handler;
    private readonly ILogger<TaskSuggestFunctions> _logger;

    public TaskSuggestFunctions(ISuggestPriorityHandler handler, ILogger<TaskSuggestFunctions> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    [Function(nameof(SuggestTaskPriority))]
    public async Task<HttpResponseData> SuggestTaskPriority(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tasks/{id:guid}/suggest")] HttpRequestData request,
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("SuggestTaskPriority invoked for {TaskId}", id);
        var updated = await _handler.HandleAsync(id, cancellationToken);
        if (updated is null)
            return request.CreateResponse(HttpStatusCode.NotFound);
        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(updated, JsonOptions), cancellationToken);
        return response;
    }
}
