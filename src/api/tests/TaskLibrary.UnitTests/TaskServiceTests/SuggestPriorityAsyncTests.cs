using Microsoft.Extensions.Logging;
using TaskLibrary.Application.Task;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.UnitTests.TaskServiceTests;

public sealed class SuggestPriorityAsyncTests
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILlmService _llmService;
    private readonly SuggestPriorityHandler _handler;

    public SuggestPriorityAsyncTests()
    {
        _taskRepository = A.Fake<ITaskRepository>();
        _llmService = A.Fake<ILlmService>();
        _handler = new SuggestPriorityHandler(_taskRepository, _llmService, A.Fake<ILogger<SuggestPriorityHandler>>());
    }

    [Fact]
    public async System.Threading.Tasks.Task SuggestPriorityAsync_WhenTaskNotFound_ReturnsNull()
    {
        A.CallTo(() => _taskRepository.FindByIdAsync(A<TaskId>._, A<CancellationToken>._))
            .Returns((Domain.Task.Task?)null);
        var result = await _handler.HandleAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);
        result.ShouldBeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task SuggestPriorityAsync_WhenLlmReturnsNull_ReturnsUnchangedTask()
    {
        var task = Domain.Task.Task.Create("Deploy", null, TaskPriority.Low, null);
        A.CallTo(() => _taskRepository.FindByIdAsync(A<TaskId>._, A<CancellationToken>._))
            .Returns(task);
        A.CallTo(() => _llmService.SuggestAsync(A<string>._, A<string?>._, A<CancellationToken>._))
            .Returns((LlmSuggestion?)null);
        var result = await _handler.HandleAsync(task.Id.Value, TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Priority.ShouldBe("Low");
        result.AiSuggestedPriority.ShouldBeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task SuggestPriorityAsync_WhenLlmReturnsSuggestion_AppliesSuggestionAndSaves()
    {
        var task = Domain.Task.Task.Create("Deploy", null, TaskPriority.Low, null);
        var suggestion = new LlmSuggestion("Critical", "DevOps", "Production is affected.");
        A.CallTo(() => _taskRepository.FindByIdAsync(A<TaskId>._, A<CancellationToken>._))
            .Returns(task);
        A.CallTo(() => _llmService.SuggestAsync(A<string>._, A<string?>._, A<CancellationToken>._))
            .Returns(suggestion);
        var result = await _handler.HandleAsync(task.Id.Value, TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.AiSuggestedPriority.ShouldBe("Critical");
        result.AiSuggestedCategory.ShouldBe("DevOps");
        result.AiReasoning.ShouldBe("Production is affected.");
        A.CallTo(() => _taskRepository.SaveTaskAsync(A<Domain.Task.Task>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}
