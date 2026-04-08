using TaskLibrary.Domain.Task;

namespace TaskLibrary.UnitTests.TaskTests.TaskTests;

/// <summary>Tests for <see cref="Task.Create"/>.</summary>
public sealed class CreateTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsTaskWithBacklogStatus()
    {
        var task = Domain.Task.Task.Create("Fix the login bug", "Users cannot log in", TaskPriority.High, "Backend");

        Assert.Equal("Fix the login bug", task.Title);
        Assert.Equal("Users cannot log in", task.Description);
        Assert.Equal(TaskStatus.Backlog, task.Status);
        Assert.Equal(TaskPriority.High, task.Priority);
        Assert.Equal("Backend", task.Category);
        Assert.NotEqual(Guid.Empty, task.Id.Value);
        Assert.False(task.AiSuggestionConfirmed);
        Assert.Null(task.AiSuggestedPriority);
    }

    [Fact]
    public void Create_WithNullCategory_ReturnsTaskWithNullCategory()
    {
        var task = Domain.Task.Task.Create("My task", null, TaskPriority.Medium, null);

        Assert.Null(task.Category);
        Assert.Null(task.Description);
    }

    [Fact]
    public void Create_WithEmptyTitle_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            Domain.Task.Task.Create("   ", "desc", TaskPriority.Low, null));

        Assert.Contains("Title cannot be empty", exception.Message);
        Assert.Equal("title", exception.ParamName);
    }

    [Fact]
    public void Create_SetsCreatedAtAndUpdatedAtToSameValue()
    {
        var before = DateTimeOffset.UtcNow;

        var task = Domain.Task.Task.Create("My task", null, TaskPriority.Medium, null);

        Assert.True(task.CreatedAt >= before);
        Assert.Equal(task.CreatedAt, task.UpdatedAt);
    }
}
