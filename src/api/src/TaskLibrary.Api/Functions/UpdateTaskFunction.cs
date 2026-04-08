using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using TaskLibrary.Application.Task;

namespace TaskLibrary.Api.Functions;

public sealed class UpdateTaskFunction
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly IUpdateTaskHandler _handler;
    private readonly ILogger<UpdateTaskFunction> _logger;

    public UpdateTaskFunction(IUpdateTaskHandler handler, ILogger<UpdateTaskFunction> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    [Function(nameof(UpdateTask))]
    public async Task<HttpResponseData> UpdateTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "tasks/{id:guid}")] HttpRequestData request,
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateTask invoked for {TaskId}", id);
        var body = await request.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body)) return request.CreateResponse(HttpStatusCode.BadRequest);
        UpdateTaskRequest? updateRequest;
        try { updateRequest = JsonSerializer.Deserialize<UpdateTaskRequest>(body, JsonOptions); }
        catch (JsonException) { return request.CreateResponse(HttpStatusCode.BadRequest); }
        if (updateRequest is null) return request.CreateResponse(HttpStatusCode.BadRequest);
        var updated = await _handler.HandleAsync(id, updateRequest, cancellationToken);
        if (updated is null) return request.CreateResponse(HttpStatusCode.NotFound);
        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(updated, JsonOptions), cancellationToken);
        return response;
    }
}
