using TaskLibrary.Api.Task;

namespace TaskLibrary.Api;

public static class EndpointRegistration
{
    public static IEndpointRouteBuilder RegisterEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.RegisterTaskEndpoints();
        return routes;
    }
}
