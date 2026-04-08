using FakeItEasy;
using TaskLibrary.Application.Task;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.UnitTests.TaskServiceTests;

/// <summary>Tests for <see cref="TaskService.CreateTaskAsync"/>.</summary>
public sealed class CreateTaskAsyncTests
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILlmService _llmService;
    private readonly TaskService _taskService;

    public CreateTaskAsyncTests()
    {
        _taskRepository = A.Fake<ITaskRepository>();
        _llmService = A.Fake<ILlmService>();
        _taskService = new TaskService(_taskRepository, _llmService);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithValidRequest_ReturnsCreatedTaskDto()
    {
        var request = new CreateTaskRequest("Fix login bug", "Users cannot log in", "High", "Backend");

        var result = await _taskService.CreateTaskAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal("Fix login bug", result.Title);
        Assert.Equal("Users cannot log in", result.Description);
        Assert.Equal("High", result.Priority);
        Assert.Equal("Backlog", result.Status);
        Assert.Equal("Backend", result.Category);
        A.CallTo(() => _taskRepository.AddAsync(A<Domain.Task.Task>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _taskService.CreateTaskAsync(null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithInvalidPriority_ThrowsArgumentException()
    {
        var request = new CreateTaskRequest("Title", null, "SuperHighInvalid", null);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _taskService.CreateTaskAsync(request, TestContext.Current.CancellationToken));
    }
}
