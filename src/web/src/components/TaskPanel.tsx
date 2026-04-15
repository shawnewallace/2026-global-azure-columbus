import { useState, useEffect } from 'react';
import type { Task, TaskStatus, TaskPriority, CreateTaskRequest, UpdateTaskRequest } from '../types/task';
import { createTask, updateTask } from '../api/tasksApi';

interface TaskPanelProps {
  task: Task | null;
  isOpen: boolean;
  onClose: () => void;
  onSaved: (task: Task) => void;
}

interface FormState {
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  category: string;
}

const DEFAULT_FORM: FormState = {
  title: '',
  description: '',
  status: 'Backlog',
  priority: 'Medium',
  category: '',
};

function hasAiSuggestion(task: Task): boolean {
  return (
    !task.aiSuggestionConfirmed &&
    (task.aiSuggestedPriority !== undefined || task.aiSuggestedCategory !== undefined)
  );
}

export default function TaskPanel({ task, isOpen, onClose, onSaved }: TaskPanelProps) {
  const [form, setForm] = useState<FormState>(DEFAULT_FORM);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (task) {
      setForm({
        title: task.title,
        description: task.description ?? '',
        status: task.status,
        priority: task.priority,
        category: task.category ?? '',
      });
    } else {
      setForm(DEFAULT_FORM);
    }
    setError(null);
  }, [task]);

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setForm(prev => ({ ...prev, [name]: value }));
  };

  const handleSave = async () => {
    if (!form.title.trim()) {
      setError('Title is required');
      return;
    }
    setSaving(true);
    setError(null);
    try {
      let saved: Task;
      if (task) {
        const req: UpdateTaskRequest = {
          title: form.title,
          description: form.description || undefined,
          status: form.status,
          priority: form.priority,
          category: form.category || undefined,
        };
        saved = await updateTask(task.id, req);
      } else {
        const req: CreateTaskRequest = {
          title: form.title,
          description: form.description || undefined,
          status: form.status,
          priority: form.priority,
          category: form.category || undefined,
        };
        saved = await createTask(req);
      }
      onSaved(saved);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Save failed');
    } finally {
      setSaving(false);
    }
  };

  const handleConfirmAi = async () => {
    if (!task) return;
    setSaving(true);
    setError(null);
    try {
      const updated = await updateTask(task.id, {
        title: task.title,
        description: task.description,
        status: task.status,
        priority: task.aiSuggestedPriority ?? task.priority,
        category: task.aiSuggestedCategory ?? task.category,
        aiSuggestionConfirmed: true,
      });
      onSaved(updated);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Confirm failed');
    } finally {
      setSaving(false);
    }
  };

  // Dismiss removes the AI badge without applying the suggested values.
  const handleDismissAi = async () => {
    if (!task) return;
    setSaving(true);
    setError(null);
    try {
      const updated = await updateTask(task.id, {
        title: task.title,
        description: task.description,
        status: task.status,
        priority: task.priority,
        category: task.category,
        aiSuggestionConfirmed: true,
      });
      onSaved(updated);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Dismiss failed');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className={`task-panel${isOpen ? ' open' : ''}`} role="dialog" aria-label="Task details">
      <div className="task-panel-header">
        <h2>{task ? 'Edit Task' : 'New Task'}</h2>
        <button className="btn-close" onClick={onClose} aria-label="Close panel">
          ✕
        </button>
      </div>

      <div className="task-panel-body">
        {error && <p className="panel-error">{error}</p>}

        <div className="form-group">
          <label htmlFor="title">Title *</label>
          <input
            id="title"
            name="title"
            type="text"
            value={form.title}
            onChange={handleChange}
            placeholder="Task title"
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="description">Description</label>
          <textarea
            id="description"
            name="description"
            value={form.description}
            onChange={handleChange}
            placeholder="Optional description"
            rows={3}
          />
        </div>

        <div className="form-row">
          <div className="form-group">
            <label htmlFor="status">Status</label>
            <select id="status" name="status" value={form.status} onChange={handleChange}>
              <option value="Backlog">Backlog</option>
              <option value="InProgress">In Progress</option>
              <option value="Done">Done</option>
            </select>
          </div>

          <div className="form-group">
            <label htmlFor="priority">Priority</label>
            <select id="priority" name="priority" value={form.priority} onChange={handleChange}>
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
              <option value="Critical">Critical</option>
            </select>
          </div>
        </div>

        <div className="form-group">
          <label htmlFor="category">Category</label>
          <input
            id="category"
            name="category"
            type="text"
            value={form.category}
            onChange={handleChange}
            placeholder="Optional category"
          />
        </div>

        {task && hasAiSuggestion(task) && (
          <div className="ai-suggestion-section">
            <h3>✨ AI Suggestion</h3>
            {task.aiSuggestedPriority && (
              <p>
                <strong>Priority:</strong> {task.aiSuggestedPriority}
              </p>
            )}
            {task.aiSuggestedCategory && (
              <p>
                <strong>Category:</strong> {task.aiSuggestedCategory}
              </p>
            )}
            {task.aiReasoning && (
              <p className="ai-reasoning">
                <em>{task.aiReasoning}</em>
              </p>
            )}
            <div className="ai-suggestion-actions">
              <button
                className="btn btn-primary"
                onClick={handleConfirmAi}
                disabled={saving}
              >
                Confirm AI Suggestion
              </button>
              <button
                className="btn btn-secondary"
                onClick={handleDismissAi}
                disabled={saving}
              >
                Dismiss
              </button>
            </div>
          </div>
        )}
      </div>

      <div className="task-panel-footer">
        <button className="btn btn-primary" onClick={handleSave} disabled={saving}>
          {saving ? 'Saving…' : 'Save'}
        </button>
        <button className="btn btn-secondary" onClick={onClose} disabled={saving}>
          Cancel
        </button>
      </div>
    </div>
  );
}
