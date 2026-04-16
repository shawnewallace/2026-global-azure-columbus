using Microsoft.Extensions.Logging;
using TaskLibrary.Application.Task;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.UnitTests.TaskServiceTests;

public sealed class UpdateTaskAsyncTests
{
    private readonly ITaskRepository _taskRepository;
    private readonly UpdateTaskHandler _handler;

    public UpdateTaskAsyncTests()
    {
        _taskRepository = A.Fake<ITaskRepository>();
        _handler = new UpdateTaskHandler(_taskRepository, A.Fake<ILogger<UpdateTaskHandler>>());
    }

    [Fact]
    public async System.Threading.Tasks.Task HandleAsync_WhenAiSuggestionConfirmedButNoSuggestionExists_DoesNotThrow()
    {
        var task = Domain.Task.Task.Create("Deploy", null, TaskPriority.Low, null);
        A.CallTo(() => _taskRepository.FindByIdAsync(A<TaskId>._, A<CancellationToken>._))
            .Returns(task);

        var request = new UpdateTaskRequest("Deploy", null, "Low", null, "Backlog", AiSuggestionConfirmed: true);

        var result = await _handler.HandleAsync(task.Id.Value, request, TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.AiSuggestionConfirmed.ShouldBeFalse();
    }

    [Fact]
    public async System.Threading.Tasks.Task HandleAsync_WhenAiSuggestionConfirmedAndSuggestionExists_PromotesSuggestion()
    {
        var task = Domain.Task.Task.Create("Deploy", null, TaskPriority.Low, null);
        task.ApplyAiSuggestion(TaskPriority.Critical, "DevOps", "Production is affected.");
        A.CallTo(() => _taskRepository.FindByIdAsync(A<TaskId>._, A<CancellationToken>._))
            .Returns(task);

        var request = new UpdateTaskRequest("Deploy", null, "Low", null, "Backlog", AiSuggestionConfirmed: true);

        var result = await _handler.HandleAsync(task.Id.Value, request, TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.AiSuggestionConfirmed.ShouldBeTrue();
        result.Priority.ShouldBe("Critical");
    }
}
