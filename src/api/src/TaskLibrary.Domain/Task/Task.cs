namespace TaskLibrary.Domain.Task;

/// <summary>
/// Task aggregate root. Encapsulates all business rules for a task
/// in the Task Library domain. Created exclusively via static factory methods.
/// </summary>
public sealed class Task
{
    public TaskId Id { get; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public string? Category { get; private set; }
    public TaskPriority? AiSuggestedPriority { get; private set; }
    public string? AiSuggestedCategory { get; private set; }
    public string? AiReasoning { get; private set; }
    public bool AiSuggestionConfirmed { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Task(
        TaskId id,
        string title,
        string? description,
        TaskStatus status,
        TaskPriority priority,
        string? category,
        DateTimeOffset createdAt)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        Category = category;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>Creates a new Task with default Backlog status.</summary>
    public static Task Create(string title, string? description, TaskPriority priority, string? category)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        return new Task(
            TaskId.NewTaskId(),
            title,
            description,
            TaskStatus.Backlog,
            priority,
            category,
            DateTimeOffset.UtcNow);
    }

    /// <summary>Reconstitutes a Task from persistence.</summary>
    public static Task Reconstitute(
        TaskId id,
        string title,
        string? description,
        TaskStatus status,
        TaskPriority priority,
        string? category,
        TaskPriority? aiSuggestedPriority,
        string? aiSuggestedCategory,
        string? aiReasoning,
        bool aiSuggestionConfirmed,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        var task = new Task(id, title, description, status, priority, category, createdAt)
        {
            AiSuggestedPriority = aiSuggestedPriority,
            AiSuggestedCategory = aiSuggestedCategory,
            AiReasoning = aiReasoning,
            AiSuggestionConfirmed = aiSuggestionConfirmed,
            UpdatedAt = updatedAt
        };

        return task;
    }

    /// <summary>Updates the mutable details of a task.</summary>
    public void UpdateDetails(string title, string? description, TaskPriority priority, string? category)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        Title = title;
        Description = description;
        Priority = priority;
        Category = category;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Advances the task status.</summary>
    public void ChangeStatus(TaskStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Records the AI suggestion for this task.</summary>
    public void ApplyAiSuggestion(TaskPriority suggestedPriority, string? suggestedCategory, string? reasoning)
    {
        AiSuggestedPriority = suggestedPriority;
        AiSuggestedCategory = suggestedCategory;
        AiReasoning = reasoning;
        AiSuggestionConfirmed = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Confirms the AI suggestion, promoting it to the active priority/category.</summary>
    public void ConfirmAiSuggestion()
    {
        if (AiSuggestedPriority is null)
            throw new InvalidOperationException("No AI suggestion available to confirm.");

        Priority = AiSuggestedPriority.Value;
        Category = AiSuggestedCategory ?? Category;
        AiSuggestionConfirmed = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
