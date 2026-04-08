using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using TaskLibrary.Application.Task;

namespace TaskLibrary.Api.Functions;

public sealed class DeleteTaskFunction
{
    private readonly IDeleteTaskHandler _handler;
    private readonly ILogger<DeleteTaskFunction> _logger;

    public DeleteTaskFunction(IDeleteTaskHandler handler, ILogger<DeleteTaskFunction> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    [Function(nameof(DeleteTask))]
    public async Task<HttpResponseData> DeleteTask(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "tasks/{id:guid}")] HttpRequestData request,
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeleteTask invoked for {TaskId}", id);
        var deleted = await _handler.HandleAsync(id, cancellationToken);
        return deleted
            ? request.CreateResponse(HttpStatusCode.NoContent)
            : request.CreateResponse(HttpStatusCode.NotFound);
    }
}
