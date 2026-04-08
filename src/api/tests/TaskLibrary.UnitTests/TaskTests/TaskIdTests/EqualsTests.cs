using Shouldly;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.UnitTests.TaskTests.TaskIdTests;

/// <summary>Tests for <see cref="TaskId"/> equality (record semantics).</summary>
public sealed class EqualsTests
{
    [Fact]
    public void TwoTaskIds_WithSameGuid_AreEqual()
    {
        var guid = Guid.NewGuid();
        var first = TaskId.Create(guid);
        var second = TaskId.Create(guid);

        first.ShouldBe(second);
    }

    [Fact]
    public void TwoTaskIds_WithDifferentGuids_AreNotEqual()
    {
        var first = TaskId.NewTaskId();
        var second = TaskId.NewTaskId();

        first.ShouldNotBe(second);
    }

    [Fact]
    public void ToString_ReturnsGuidString()
    {
        var guid = Guid.NewGuid();
        var taskId = TaskId.Create(guid);

        taskId.ToString().ShouldBe(guid.ToString());
    }
}
