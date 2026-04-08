using FakeItEasy;
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
        _handler = new SuggestPriorityHandler(_taskRepository, _llmService);
    }

    [Fact]
    public async System.Threading.Tasks.Task SuggestPriorityAsync_WhenTaskNotFound_ReturnsNull()
    {
        A.CallTo(() => _taskRepository.FindByIdAsync(A<TaskId>._, A<CancellationToken>._))
            .Returns((Domain.Task.Task?)null);
        var result = await _handler.HandleAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);
        Assert.Null(result);
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
        Assert.NotNull(result);
        Assert.Equal("Low", result.Priority);
        Assert.Null(result.AiSuggestedPriority);
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
        Assert.NotNull(result);
        Assert.Equal("Critical", result.AiSuggestedPriority);
        Assert.Equal("DevOps", result.AiSuggestedCategory);
        Assert.Equal("Production is affected.", result.AiReasoning);
        A.CallTo(() => _taskRepository.SaveTaskAsync(A<Domain.Task.Task>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}
