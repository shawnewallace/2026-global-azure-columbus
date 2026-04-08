import { useState, useEffect, useCallback } from 'react';
import type { Task, TaskStatus, TaskPriority } from '../types/task';
import { getTasks, deleteTask, suggestForTask } from '../api/tasksApi';
import AiBadge from './AiBadge';

interface TaskListProps {
  selectedTaskId: string | null;
  onSelectTask: (task: Task | null) => void;
  onCreateNew: () => void;
  refreshToken: number;
}

const STATUS_OPTIONS: Array<{ value: '' | TaskStatus; label: string }> = [
  { value: '', label: 'All Statuses' },
  { value: 'Backlog', label: 'Backlog' },
  { value: 'InProgress', label: 'In Progress' },
  { value: 'Done', label: 'Done' },
];

const PRIORITY_OPTIONS: Array<{ value: '' | TaskPriority; label: string }> = [
  { value: '', label: 'All Priorities' },
  { value: 'Low', label: 'Low' },
  { value: 'Medium', label: 'Medium' },
  { value: 'High', label: 'High' },
  { value: 'Critical', label: 'Critical' },
];

function hasAiSuggestion(task: Task): boolean {
  return (
    !task.aiSuggestionConfirmed &&
    (task.aiSuggestedPriority !== undefined || task.aiSuggestedCategory !== undefined)
  );
}

export default function TaskList({
  selectedTaskId,
  onSelectTask,
  onCreateNew,
  refreshToken,
}: TaskListProps) {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [filterStatus, setFilterStatus] = useState<'' | TaskStatus>('');
  const [filterPriority, setFilterPriority] = useState<'' | TaskPriority>('');
  const [filterCategory, setFilterCategory] = useState('');

  const loadTasks = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await getTasks({
        status: filterStatus || undefined,
        priority: filterPriority || undefined,
        category: filterCategory || undefined,
      });
      setTasks(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load tasks');
    } finally {
      setLoading(false);
    }
  }, [filterStatus, filterPriority, filterCategory, refreshToken]);

  useEffect(() => {
    loadTasks();
  }, [loadTasks]);

  const handleDelete = async (task: Task, event: React.MouseEvent) => {
    event.stopPropagation();
    if (!confirm(`Delete "${task.title}"?`)) return;
    try {
      await deleteTask(task.id);
      if (selectedTaskId === task.id) onSelectTask(null);
      await loadTasks();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Delete failed');
    }
  };

  const handleSuggest = async (task: Task, event: React.MouseEvent) => {
    event.stopPropagation();
    try {
      const updated = await suggestForTask(task.id);
      setTasks(prev => prev.map(t => (t.id === updated.id ? updated : t)));
    } catch (err) {
      alert(err instanceof Error ? err.message : 'AI suggestion failed');
    }
  };

  return (
    <div className="task-list">
      <div className="task-list-header">
        <h2>Tasks</h2>
        <button className="btn btn-primary" onClick={onCreateNew}>
          + New Task
        </button>
      </div>

      <div className="filter-bar">
        <select
          value={filterStatus}
          onChange={e => setFilterStatus(e.target.value as '' | TaskStatus)}
          aria-label="Filter by status"
        >
          {STATUS_OPTIONS.map(o => (
            <option key={o.value} value={o.value}>
              {o.label}
            </option>
          ))}
        </select>

        <select
          value={filterPriority}
          onChange={e => setFilterPriority(e.target.value as '' | TaskPriority)}
          aria-label="Filter by priority"
        >
          {PRIORITY_OPTIONS.map(o => (
            <option key={o.value} value={o.value}>
              {o.label}
            </option>
          ))}
        </select>

        <input
          type="text"
          placeholder="Filter by category…"
          value={filterCategory}
          onChange={e => setFilterCategory(e.target.value)}
          aria-label="Filter by category"
        />
      </div>

      {loading && <p className="state-message">Loading…</p>}
      {error && <p className="state-message error">{error}</p>}

      {!loading && !error && tasks.length === 0 && (
        <p className="state-message">No tasks found. Create one!</p>
      )}

      {!loading && tasks.length > 0 && (
        <table className="task-table">
          <thead>
            <tr>
              <th>Title</th>
              <th>Status</th>
              <th>Priority</th>
              <th>Category</th>
              <th>AI</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {tasks.map(task => (
              <tr
                key={task.id}
                className={`task-row${selectedTaskId === task.id ? ' selected' : ''}`}
                onClick={() => onSelectTask(task)}
              >
                <td className="task-title">{task.title}</td>
                <td>
                  <span className={`badge status-${task.status.toLowerCase()}`}>{task.status}</span>
                </td>
                <td>
                  <span className={`badge priority-${task.priority.toLowerCase()}`}>
                    {task.priority}
                  </span>
                </td>
                <td>{task.category ?? '—'}</td>
                <td>
                  {hasAiSuggestion(task) && (
                    <AiBadge
                      suggestedPriority={task.aiSuggestedPriority}
                      suggestedCategory={task.aiSuggestedCategory}
                    />
                  )}
                </td>
                <td className="task-actions">
                  <button
                    className="btn btn-sm btn-secondary"
                    onClick={e => {
                      e.stopPropagation();
                      onSelectTask(task);
                    }}
                  >
                    Edit
                  </button>
                  <button
                    className="btn btn-sm btn-ghost"
                    onClick={e => handleSuggest(task, e)}
                    title="Get AI suggestion"
                  >
                    ✨
                  </button>
                  <button
                    className="btn btn-sm btn-danger"
                    onClick={e => handleDelete(task, e)}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
