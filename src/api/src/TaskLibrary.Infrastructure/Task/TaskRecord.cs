namespace TaskLibrary.Infrastructure.Task;

/// <summary>
/// Flat persistence model for the tasks table. Kept separate from the
/// domain aggregate to respect the anti-corruption layer principle.
/// </summary>
public sealed class TaskRecord
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Backlog";
    public string Priority { get; set; } = "Medium";
    public string? Category { get; set; }
    public string? AiSuggestedPriority { get; set; }
    public string? AiSuggestedCategory { get; set; }
    public string? AiReasoning { get; set; }
    public bool AiSuggestionConfirmed { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
