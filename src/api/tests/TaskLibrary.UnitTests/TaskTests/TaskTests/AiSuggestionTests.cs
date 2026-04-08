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

        task.AiSuggestedPriority.ShouldBe(TaskPriority.Critical);
        task.AiSuggestedCategory.ShouldBe("DevOps");
        task.AiReasoning.ShouldBe("Critical because prod is affected.");
        task.AiSuggestionConfirmed.ShouldBeFalse();
        task.Priority.ShouldBe(TaskPriority.Low);
    }

    [Fact]
    public void ConfirmAiSuggestion_PromotesSuggestedPriorityAndCategory()
    {
        var task = Domain.Task.Task.Create("Deploy service", null, TaskPriority.Low, null);
        task.ApplyAiSuggestion(TaskPriority.Critical, "DevOps", "Critical because prod is affected.");

        task.ConfirmAiSuggestion();

        task.Priority.ShouldBe(TaskPriority.Critical);
        task.Category.ShouldBe("DevOps");
        task.AiSuggestionConfirmed.ShouldBeTrue();
    }

    [Fact]
    public void ConfirmAiSuggestion_WithoutSuggestion_ThrowsInvalidOperationException()
    {
        var task = Domain.Task.Task.Create("Deploy service", null, TaskPriority.Low, null);

        Should.Throw<InvalidOperationException>(() => task.ConfirmAiSuggestion());
    }
}
