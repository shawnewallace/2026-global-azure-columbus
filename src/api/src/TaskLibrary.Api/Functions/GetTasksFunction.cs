using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using TaskLibrary.Application.Task;

namespace TaskLibrary.Api.Functions;

public sealed class GetTasksFunction
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly IListTasksHandler _handler;
    private readonly ILogger<GetTasksFunction> _logger;

    public GetTasksFunction(IListTasksHandler handler, ILogger<GetTasksFunction> logger)
    {
        _handler = handler;
        _logger = logger;
    }

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
        var tasks = await _handler.HandleAsync(status, priority, category, cancellationToken);
        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(tasks, JsonOptions), cancellationToken);
        return response;
    }
}
