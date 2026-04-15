using Microsoft.Extensions.Logging;
using TaskLibrary.Application.Task;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.UnitTests.TaskServiceTests;

public sealed class GetTaskAsyncTests
{
    private readonly ITaskRepository _taskRepository;
    private readonly GetTaskHandler _handler;

    public GetTaskAsyncTests()
    {
        _taskRepository = A.Fake<ITaskRepository>();
        _handler = new GetTaskHandler(_taskRepository, A.Fake<ILogger<GetTaskHandler>>());
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskAsync_WhenTaskExists_ReturnsTaskDto()
    {
        var task = Domain.Task.Task.Create("My task", null, TaskPriority.Medium, null);
        A.CallTo(() => _taskRepository.FindByIdAsync(A<TaskId>._, A<CancellationToken>._))
            .Returns(task);
        var result = await _handler.HandleAsync(task.Id.Value, TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Title.ShouldBe("My task");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskAsync_WhenTaskDoesNotExist_ReturnsNull()
    {
        A.CallTo(() => _taskRepository.FindByIdAsync(A<TaskId>._, A<CancellationToken>._))
            .Returns((Domain.Task.Task?)null);
        var result = await _handler.HandleAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);
        result.ShouldBeNull();
    }
}
