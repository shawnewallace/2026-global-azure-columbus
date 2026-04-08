import { useState, useCallback } from 'react';
import type { Task } from './types/task';
import TaskList from './components/TaskList';
import TaskPanel from './components/TaskPanel';
import './App.css';

export default function App() {
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const [panelOpen, setPanelOpen] = useState(false);
  const [refreshToken, setRefreshToken] = useState(0);

  const handleSelectTask = useCallback((task: Task | null) => {
    setSelectedTask(task);
    setPanelOpen(task !== null);
  }, []);

  const handleCreateNew = useCallback(() => {
    setSelectedTask(null);
    setPanelOpen(true);
  }, []);

  const handleClose = useCallback(() => {
    setPanelOpen(false);
    setSelectedTask(null);
  }, []);

  const handleSaved = useCallback((saved: Task) => {
    setSelectedTask(saved);
    setRefreshToken(t => t + 1);
  }, []);

  return (
    <div className="app">
      <header className="app-header">
        <span role="img" aria-label="tasks">📋</span>
        <h1>Task Library</h1>
      </header>

      <main className="app-content">
        <TaskList
          selectedTaskId={selectedTask?.id ?? null}
          onSelectTask={handleSelectTask}
          onCreateNew={handleCreateNew}
          refreshToken={refreshToken}
        />
      </main>

      <div
        className={`panel-overlay${panelOpen ? ' visible' : ''}`}
        onClick={handleClose}
        aria-hidden="true"
      />

      <TaskPanel
        task={selectedTask}
        isOpen={panelOpen}
        onClose={handleClose}
        onSaved={handleSaved}
      />
    </div>
  );
}
