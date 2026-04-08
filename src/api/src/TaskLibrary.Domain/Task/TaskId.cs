namespace TaskLibrary.Domain.Task;

/// <summary>Strongly-typed identifier for a Task aggregate.</summary>
public sealed record TaskId
{
    public Guid Value { get; }

    private TaskId(Guid value)
    {
        Value = value;
    }

    /// <summary>Creates a new unique TaskId.</summary>
    public static TaskId NewTaskId() => new(Guid.NewGuid());

    /// <summary>Creates a TaskId from an existing Guid.</summary>
    public static TaskId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("TaskId value cannot be empty.", nameof(value));

        return new TaskId(value);
    }

    public override string ToString() => Value.ToString();
}
