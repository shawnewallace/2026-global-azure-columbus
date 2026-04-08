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

        Assert.Equal(guid, taskId.Value);
    }

    [Fact]
    public void Create_WithEmptyGuid_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => TaskId.Create(Guid.Empty));

        Assert.Contains("TaskId value cannot be empty", exception.Message);
        Assert.Equal("value", exception.ParamName);
    }
}
