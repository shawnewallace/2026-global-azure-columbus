using Shouldly;
using TaskLibrary.Domain.Task;

namespace TaskLibrary.UnitTests.TaskTests.TaskIdTests;

/// <summary>Tests for <see cref="TaskId.Create"/>.</summary>
public sealed class CreateTests
{
    [Fact]
    public void Create_WithValidGuid_ReturnsTaskIdWithSameValue()
    {
        var guid = Guid.NewGuid();

        var taskId = TaskId.Create(guid);

        taskId.Value.ShouldBe(guid);
    }

    [Fact]
    public void Create_WithEmptyGuid_ThrowsArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() => TaskId.Create(Guid.Empty));

        exception.Message.ShouldContain("TaskId value cannot be empty");
        exception.ParamName.ShouldBe("value");
    }
}
