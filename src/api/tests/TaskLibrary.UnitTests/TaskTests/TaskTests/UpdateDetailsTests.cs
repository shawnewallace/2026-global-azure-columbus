using Shouldly;
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

        task.Title.ShouldBe("Updated Title");
        task.Description.ShouldBe("New desc");
        task.Priority.ShouldBe(TaskPriority.Critical);
        task.Category.ShouldBe("Frontend");
        task.UpdatedAt.ShouldBeGreaterThanOrEqualTo(originalUpdatedAt);
    }

    [Fact]
    public void UpdateDetails_WithEmptyTitle_ThrowsArgumentException()
    {
        var task = Domain.Task.Task.Create("Original", null, TaskPriority.Low, null);

        Should.Throw<ArgumentException>(() => task.UpdateDetails("", null, TaskPriority.Low, null));
    }
}
