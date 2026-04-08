using TaskLibrary.Domain.Task;

namespace TaskLibrary.UnitTests.TaskTests.TaskTests;

/// <summary>Tests for <see cref="Task.UpdateDetails"/>.</summary>
public sealed class UpdateDetailsTests
{
    [Fact]
    public void UpdateDetails_WithValidInputs_UpdatesProperties()
    {
        var task = Domain.Task.Task.Create("Original", null, TaskPriority.Low, null);
        var originalUpdatedAt = task.UpdatedAt;

        task.UpdateDetails("Updated Title", "New desc", TaskPriority.Critical, "Frontend");

        Assert.Equal("Updated Title", task.Title);
        Assert.Equal("New desc", task.Description);
        Assert.Equal(TaskPriority.Critical, task.Priority);
        Assert.Equal("Frontend", task.Category);
        Assert.True(task.UpdatedAt >= originalUpdatedAt);
    }

    [Fact]
    public void UpdateDetails_WithEmptyTitle_ThrowsArgumentException()
    {
        var task = Domain.Task.Task.Create("Original", null, TaskPriority.Low, null);

        Assert.Throws<ArgumentException>(() => task.UpdateDetails("", null, TaskPriority.Low, null));
    }
}
