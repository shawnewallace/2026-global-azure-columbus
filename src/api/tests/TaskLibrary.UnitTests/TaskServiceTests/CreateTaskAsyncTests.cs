using FakeItEasy;
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
        _handler = new CreateTaskHandler(_taskRepository);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithValidRequest_ReturnsCreatedTaskDto()
    {
        var request = new CreateTaskRequest("Fix login bug", "Users cannot log in", "High", "Backend");
        var result = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);
        Assert.Equal("Fix login bug", result.Title);
        Assert.Equal("Users cannot log in", result.Description);
        Assert.Equal("High", result.Priority);
        Assert.Equal("Backlog", result.Status);
        Assert.Equal("Backend", result.Category);
        A.CallTo(() => _taskRepository.SaveNewTaskAsync(A<Domain.Task.Task>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.HandleAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithInvalidPriority_ThrowsArgumentException()
    {
        var request = new CreateTaskRequest("Title", null, "SuperHighInvalid", null);
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.HandleAsync(request, TestContext.Current.CancellationToken));
    }
}
