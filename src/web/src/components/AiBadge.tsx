import type { TaskPriority } from '../types/task';

interface AiBadgeProps {
  suggestedPriority?: TaskPriority;
  suggestedCategory?: string;
}

export default function AiBadge({ suggestedPriority, suggestedCategory }: AiBadgeProps) {
  const parts: string[] = [];
  if (suggestedPriority) parts.push(suggestedPriority);
  if (suggestedCategory) parts.push(suggestedCategory);

  return (
    <span className="ai-badge" title={`AI suggests: ${parts.join(', ')}`}>
      ✨ AI Suggested
    </span>
  );
}
