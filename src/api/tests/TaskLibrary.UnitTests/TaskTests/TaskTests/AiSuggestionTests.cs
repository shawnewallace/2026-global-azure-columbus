using TaskLibrary.Domain.Task;

namespace TaskLibrary.UnitTests.TaskTests.TaskTests;

/// <summary>Tests for <see cref="Task.ApplyAiSuggestion"/> and <see cref="Task.ConfirmAiSuggestion"/>.</summary>
public sealed class AiSuggestionTests
{
    [Fact]
    public void ApplyAiSuggestion_SetsSuggestedFieldsAndClearsConfirmation()
    {
        var task = Domain.Task.Task.Create("Deploy service", null, TaskPriority.Low, null);

        task.ApplyAiSuggestion(TaskPriority.Critical, "DevOps", "Critical because prod is affected.");

        Assert.Equal(TaskPriority.Critical, task.AiSuggestedPriority);
        Assert.Equal("DevOps", task.AiSuggestedCategory);
        Assert.Equal("Critical because prod is affected.", task.AiReasoning);
        Assert.False(task.AiSuggestionConfirmed);
        Assert.Equal(TaskPriority.Low, task.Priority);
    }

    [Fact]
    public void ConfirmAiSuggestion_PromotesSuggestedPriorityAndCategory()
    {
        var task = Domain.Task.Task.Create("Deploy service", null, TaskPriority.Low, null);
        task.ApplyAiSuggestion(TaskPriority.Critical, "DevOps", "Critical because prod is affected.");

        task.ConfirmAiSuggestion();

        Assert.Equal(TaskPriority.Critical, task.Priority);
        Assert.Equal("DevOps", task.Category);
        Assert.True(task.AiSuggestionConfirmed);
    }

    [Fact]
    public void ConfirmAiSuggestion_WithoutSuggestion_ThrowsInvalidOperationException()
    {
        var task = Domain.Task.Task.Create("Deploy service", null, TaskPriority.Low, null);

        Assert.Throws<InvalidOperationException>(() => task.ConfirmAiSuggestion());
    }
}
