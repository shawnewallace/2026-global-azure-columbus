using TaskLibrary.Domain.Task;

namespace TaskLibrary.UnitTests.TaskTests.TaskTests;

/// <summary>Tests for <see cref="Task.Create"/>.</summary>
public sealed class CreateTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsTaskWithBacklogStatus()
    {
        var task = Domain.Task.Task.Create("Fix the login bug", "Users cannot log in", TaskPriority.High, "Backend");

        task.Title.ShouldBe("Fix the login bug");
        task.Description.ShouldBe("Users cannot log in");
        task.Status.ShouldBe(TaskStatus.Backlog);
        task.Priority.ShouldBe(TaskPriority.High);
        task.Category.ShouldBe("Backend");
        task.Id.Value.ShouldNotBe(Guid.Empty);
        task.AiSuggestionConfirmed.ShouldBeFalse();
        task.AiSuggestedPriority.ShouldBeNull();
    }

    [Fact]
    public void Create_WithNullCategory_ReturnsTaskWithNullCategory()
    {
        var task = Domain.Task.Task.Create("My task", null, TaskPriority.Medium, null);

        task.Category.ShouldBeNull();
        task.Description.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyTitle_ThrowsArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() =>
            Domain.Task.Task.Create("   ", "desc", TaskPriority.Low, null));

        exception.Message.ShouldContain("Title cannot be empty");
        exception.ParamName.ShouldBe("title");
    }

    [Fact]
    public void Create_SetsCreatedAtAndUpdatedAtToSameValue()
    {
        var before = DateTimeOffset.UtcNow;

        var task = Domain.Task.Task.Create("My task", null, TaskPriority.Medium, null);

        task.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
        task.CreatedAt.ShouldBe(task.UpdatedAt);
    }
}
