using System.Net.Http;

namespace TaskLibrary.IntegrationTests.CorrelationIdMiddlewareTests;

public sealed class CorrelationIdMiddlewareTests : IClassFixture<TaskLibraryWebApplicationFactory>
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    // Use the OpenAPI spec endpoint — zero DB dependency, always 200.
    private const string ProbePath = "/openapi/v1.json";

    private readonly HttpClient _client;

    public CorrelationIdMiddlewareTests(TaskLibraryWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async System.Threading.Tasks.Task WhenCorrelationIdHeaderProvided_ResponseContainsSameId()
    {
        var correlationId = Guid.NewGuid().ToString();
        var request = new HttpRequestMessage(HttpMethod.Get, ProbePath);
        request.Headers.Add(CorrelationIdHeader, correlationId);

        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        response.Headers.TryGetValues(CorrelationIdHeader, out var values).ShouldBeTrue();
        values!.Single().ShouldBe(correlationId);
    }

    [Fact]
    public async System.Threading.Tasks.Task WhenCorrelationIdHeaderAbsent_ResponseContainsNewGeneratedId()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ProbePath);

        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        response.Headers.TryGetValues(CorrelationIdHeader, out var values).ShouldBeTrue();
        Guid.TryParse(values!.Single(), out _).ShouldBeTrue();
    }
}
