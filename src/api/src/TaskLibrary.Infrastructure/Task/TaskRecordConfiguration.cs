using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskLibrary.Infrastructure.Task;

/// <summary>
/// EF Core entity type configuration for <see cref="TaskRecord"/>.
/// Maps the domain model to the tasks table defined in FR-01.
/// </summary>
public sealed class TaskRecordConfiguration : IEntityTypeConfiguration<TaskRecord>
{
    public void Configure(EntityTypeBuilder<TaskRecord> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(t => t.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
        builder.Property(t => t.Description).HasColumnName("description");
        builder.Property(t => t.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Backlog");
        builder.Property(t => t.Priority).HasColumnName("priority").HasMaxLength(20).IsRequired().HasDefaultValue("Medium");
        builder.Property(t => t.Category).HasColumnName("category").HasMaxLength(100);
        builder.Property(t => t.AiSuggestedPriority).HasColumnName("ai_suggested_priority").HasMaxLength(20);
        builder.Property(t => t.AiSuggestedCategory).HasColumnName("ai_suggested_category").HasMaxLength(100);
        builder.Property(t => t.AiReasoning).HasColumnName("ai_reasoning");
        builder.Property(t => t.AiSuggestionConfirmed).HasColumnName("ai_suggestion_confirmed").HasDefaultValue(false);
        builder.Property(t => t.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at").IsRequired();
    }
}
