export type TaskStatus = 'Backlog' | 'InProgress' | 'Done';
export type TaskPriority = 'Low' | 'Medium' | 'High' | 'Critical';

export interface Task {
  id: string;
  title: string;
  description?: string;
  status: TaskStatus;
  priority: TaskPriority;
  category?: string;
  aiSuggestedPriority?: TaskPriority;
  aiSuggestedCategory?: string;
  aiReasoning?: string;
  aiSuggestionConfirmed: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  status?: TaskStatus;
  priority?: TaskPriority;
  category?: string;
}

export interface UpdateTaskRequest {
  title?: string;
  description?: string;
  status?: TaskStatus;
  priority?: TaskPriority;
  category?: string;
  aiSuggestionConfirmed?: boolean;
}
