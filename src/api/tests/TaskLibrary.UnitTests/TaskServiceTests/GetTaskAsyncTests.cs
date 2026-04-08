using FakeItEasy;
using TaskLibrary.Application.Task;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.UnitTests.TaskServiceTests;

/// <summary>Tests for <see cref="TaskService.GetTaskAsync"/>.</summary>
public sealed class GetTaskAsyncTests
{
    private readonly ITaskRepository _taskRepository;
    private readonly TaskService _taskService;

    public GetTaskAsyncTests()
    {
        _taskRepository = A.Fake<ITaskRepository>();
        var llmService = A.Fake<ILlmService>();
        _taskService = new TaskService(_taskRepository, llmService);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskAsync_WhenTaskExists_ReturnsTaskDto()
    {
        var task = Domain.Task.Task.Create("My task", null, TaskPriority.Medium, null);
        A.CallTo(() => _taskRepository.FindByIdAsync(A<TaskId>._, A<CancellationToken>._))
            .Returns(task);

        var result = await _taskService.GetTaskAsync(task.Id.Value, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal("My task", result.Title);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskAsync_WhenTaskDoesNotExist_ReturnsNull()
    {
        A.CallTo(() => _taskRepository.FindByIdAsync(A<TaskId>._, A<CancellationToken>._))
            .Returns((Domain.Task.Task?)null);

        var result = await _taskService.GetTaskAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        Assert.Null(result);
    }
}
