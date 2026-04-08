using TaskLibrary.Domain.Task;

namespace TaskLibrary.UnitTests.TaskTests.TaskIdTests;

/// <summary>Tests for <see cref="TaskId.NewTaskId"/>.</summary>
public sealed class NewTaskIdTests
{
    [Fact]
    public void NewTaskId_ReturnsNonEmptyGuid()
    {
        var taskId = TaskId.NewTaskId();

        Assert.NotEqual(Guid.Empty, taskId.Value);
    }

    [Fact]
    public void NewTaskId_CalledTwice_ReturnsDifferentIds()
    {
        var first = TaskId.NewTaskId();
        var second = TaskId.NewTaskId();

        Assert.NotEqual(first, second);
    }
}
