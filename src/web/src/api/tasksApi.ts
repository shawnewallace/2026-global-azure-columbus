import type { Task, CreateTaskRequest, UpdateTaskRequest, TaskStatus, TaskPriority } from '../types/task';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:7071';

async function fetchJson<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json', ...init?.headers },
    ...init,
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(`API error ${response.status}: ${text}`);
  }

  if (response.status === 204) {
    return undefined as unknown as T;
  }

  return response.json() as Promise<T>;
}

export interface TaskFilters {
  status?: TaskStatus;
  priority?: TaskPriority;
  category?: string;
}

export function getTasks(filters?: TaskFilters): Promise<Task[]> {
  const params = new URLSearchParams();
  if (filters?.status) params.set('status', filters.status);
  if (filters?.priority) params.set('priority', filters.priority);
  if (filters?.category) params.set('category', filters.category);

  const query = params.toString() ? `?${params.toString()}` : '';
  return fetchJson<Task[]>(`/api/tasks${query}`);
}

export function getTask(id: string): Promise<Task> {
  return fetchJson<Task>(`/api/tasks/${id}`);
}

export function createTask(request: CreateTaskRequest): Promise<Task> {
  return fetchJson<Task>('/api/tasks', {
    method: 'POST',
    body: JSON.stringify(request),
  });
}

export function updateTask(id: string, request: UpdateTaskRequest): Promise<Task> {
  return fetchJson<Task>(`/api/tasks/${id}`, {
    method: 'PUT',
    body: JSON.stringify(request),
  });
}

export function deleteTask(id: string): Promise<void> {
  return fetchJson<void>(`/api/tasks/${id}`, { method: 'DELETE' });
}

export function suggestForTask(id: string): Promise<Task> {
  return fetchJson<Task>(`/api/tasks/${id}/suggest`, { method: 'POST' });
}
