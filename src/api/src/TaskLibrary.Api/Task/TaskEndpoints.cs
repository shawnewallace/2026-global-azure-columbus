using TaskLibrary.Application.Task;

namespace TaskLibrary.Api.Task;

public static class TaskEndpoints
{
    public static IEndpointRouteBuilder RegisterTaskEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/tasks").WithTags("Tasks");

        group.MapGet("/", async (
            string? status, string? priority, string? category,
            IListTasksHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.HandleAsync(status, priority, category, ct)));

        group.MapGet("/{id:guid}", async (
            Guid id, IGetTaskHandler handler, CancellationToken ct) =>
        {
            var task = await handler.HandleAsync(id, ct);
            return task is null ? Results.NotFound() : Results.Ok(task);
        });

        group.MapPost("/", async (
            CreateTaskRequest request, ICreateTaskHandler handler, CancellationToken ct) =>
        {
            try
            {
                var created = await handler.HandleAsync(request, ct);
                return Results.Created($"/api/tasks/{created.Id}", created);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapPut("/{id:guid}", async (
            Guid id, UpdateTaskRequest request, IUpdateTaskHandler handler, CancellationToken ct) =>
        {
            try
            {
                var updated = await handler.HandleAsync(id, request, ct);
                return updated is null ? Results.NotFound() : Results.Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapDelete("/{id:guid}", async (
            Guid id, IDeleteTaskHandler handler, CancellationToken ct) =>
        {
            var deleted = await handler.HandleAsync(id, ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        group.MapPost("/{id:guid}/suggest", async (
            Guid id, ISuggestPriorityHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.HandleAsync(id, ct);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        return routes;
    }
}
