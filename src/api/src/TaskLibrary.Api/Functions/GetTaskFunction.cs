using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using TaskLibrary.Application.Task;

namespace TaskLibrary.Api.Functions;

public sealed class GetTaskFunction
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly IGetTaskHandler _handler;
    private readonly ILogger<GetTaskFunction> _logger;

    public GetTaskFunction(IGetTaskHandler handler, ILogger<GetTaskFunction> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    [Function(nameof(GetTask))]
    public async Task<HttpResponseData> GetTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tasks/{id:guid}")] HttpRequestData request,
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetTask invoked for {TaskId}", id);
        var task = await _handler.HandleAsync(id, cancellationToken);
        if (task is null) return request.CreateResponse(HttpStatusCode.NotFound);
        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(task, JsonOptions), cancellationToken);
        return response;
    }
}
