using TaskLibrary.Domain.Task;

namespace TaskLibrary.Infrastructure.Task;

internal static class TaskEntityMapper
{
    internal static Domain.Task.Task ToDomain(TaskRecord record)
    {
        var taskId = TaskId.Create(record.Id);
        if (!Enum.TryParse<Domain.Task.TaskStatus>(record.Status, out var status))
            throw new InvalidOperationException($"Unknown task status value: '{record.Status}'.");
        if (!Enum.TryParse<Domain.Task.TaskPriority>(record.Priority, out var priority))
            throw new InvalidOperationException($"Unknown task priority value: '{record.Priority}'.");
        Domain.Task.TaskPriority? aiPriority = null;
        if (record.AiSuggestedPriority is not null)
        {
            if (!Enum.TryParse<Domain.Task.TaskPriority>(record.AiSuggestedPriority, out var parsedAiPriority))
                throw new InvalidOperationException($"Unknown AI priority value: '{record.AiSuggestedPriority}'.");
            aiPriority = parsedAiPriority;
        }
        return Domain.Task.Task.Reconstitute(taskId, record.Title, record.Description, status, priority,
            record.Category, aiPriority, record.AiSuggestedCategory, record.AiReasoning,
            record.AiSuggestionConfirmed, record.CreatedAt, record.UpdatedAt);
    }

    internal static TaskRecord ToRecord(Domain.Task.Task task) => new()
    {
        Id = task.Id.Value,
        Title = task.Title,
        Description = task.Description,
        Status = task.Status.ToString(),
        Priority = task.Priority.ToString(),
        Category = task.Category,
        AiSuggestedPriority = task.AiSuggestedPriority?.ToString(),
        AiSuggestedCategory = task.AiSuggestedCategory,
        AiReasoning = task.AiReasoning,
        AiSuggestionConfirmed = task.AiSuggestionConfirmed,
        CreatedAt = task.CreatedAt,
        UpdatedAt = task.UpdatedAt
    };

    internal static void UpdateRecord(Domain.Task.Task task, TaskRecord record)
    {
        record.Title = task.Title;
        record.Description = task.Description;
        record.Status = task.Status.ToString();
        record.Priority = task.Priority.ToString();
        record.Category = task.Category;
        record.AiSuggestedPriority = task.AiSuggestedPriority?.ToString();
        record.AiSuggestedCategory = task.AiSuggestedCategory;
        record.AiReasoning = task.AiReasoning;
        record.AiSuggestionConfirmed = task.AiSuggestionConfirmed;
        record.UpdatedAt = task.UpdatedAt;
    }
}
