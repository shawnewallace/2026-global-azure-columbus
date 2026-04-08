using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using TaskLibrary.Application.Task;

namespace TaskLibrary.Api.Functions;

public sealed class CreateTaskFunction
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly ICreateTaskHandler _handler;
    private readonly ILogger<CreateTaskFunction> _logger;

    public CreateTaskFunction(ICreateTaskHandler handler, ILogger<CreateTaskFunction> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    [Function(nameof(CreateTask))]
    public async Task<HttpResponseData> CreateTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tasks")] HttpRequestData request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateTask invoked");
        var body = await request.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body)) return request.CreateResponse(HttpStatusCode.BadRequest);
        CreateTaskRequest? createRequest;
        try { createRequest = JsonSerializer.Deserialize<CreateTaskRequest>(body, JsonOptions); }
        catch (JsonException) { return request.CreateResponse(HttpStatusCode.BadRequest); }
        if (createRequest is null) return request.CreateResponse(HttpStatusCode.BadRequest);
        var created = await _handler.HandleAsync(createRequest, cancellationToken);
        var response = request.CreateResponse(HttpStatusCode.Created);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(created, JsonOptions), cancellationToken);
        return response;
    }
}
