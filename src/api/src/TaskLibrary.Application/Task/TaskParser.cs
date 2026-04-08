namespace TaskLibrary.Application.Task;

internal static class TaskParser
{
    internal static Domain.Task.TaskPriority ParsePriority(string priority)
    {
        if (!Enum.TryParse<Domain.Task.TaskPriority>(priority, ignoreCase: true, out var parsed))
            throw new ArgumentException($"Invalid priority value: '{priority}'.", nameof(priority));
        return parsed;
    }

    internal static Domain.Task.TaskStatus ParseStatus(string status)
    {
        if (!Enum.TryParse<Domain.Task.TaskStatus>(status, ignoreCase: true, out var parsed))
            throw new ArgumentException($"Invalid status value: '{status}'.", nameof(status));
        return parsed;
    }
}
