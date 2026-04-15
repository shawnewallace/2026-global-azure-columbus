using Microsoft.Extensions.Logging;
using TaskLibrary.Application.Task;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.UnitTests.TaskServiceTests;

public sealed class CreateTaskAsyncTests
{
    private readonly ITaskRepository _taskRepository;
    private readonly CreateTaskHandler _handler;

    public CreateTaskAsyncTests()
    {
        _taskRepository = A.Fake<ITaskRepository>();
        _handler = new CreateTaskHandler(_taskRepository, A.Fake<ILogger<CreateTaskHandler>>());
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithValidRequest_ReturnsCreatedTaskDto()
    {
        var request = new CreateTaskRequest("Fix login bug", "Users cannot log in", "High", "Backend");
        var result = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);
        result.Title.ShouldBe("Fix login bug");
        result.Description.ShouldBe("Users cannot log in");
        result.Priority.ShouldBe("High");
        result.Status.ShouldBe("Backlog");
        result.Category.ShouldBe("Backend");
        A.CallTo(() => _taskRepository.SaveNewTaskAsync(A<Domain.Task.Task>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _handler.HandleAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithInvalidPriority_ThrowsArgumentException()
    {
        var request = new CreateTaskRequest("Title", null, "SuperHighInvalid", null);
        await Should.ThrowAsync<ArgumentException>(() =>
            _handler.HandleAsync(request, TestContext.Current.CancellationToken));
    }
}
