import { useEffect, useState } from "react";
import { TaskForm } from "./TaskForm";
import { createTask, deleteTask, getTaskStates, getTasks, updateTask } from "./taskApi";
import { useAppSelector } from "../../hooks";
import { selectToken } from "../auth/authSlice";
import type { TaskFilters, TaskFormValues, TaskItem, TaskState } from "./taskTypes";
import { FeedbackMessage } from "../../shared/components/FeedbackMessage";
import { ConfirmDialog } from "../../shared/components/ConfirmDialog";
import { TaskDetailDialog } from "../../shared/components/TaskDetailDialog";

const initialFilters: TaskFilters = {
  Search: "",
  StateId: "",
  DueDateFrom: "",
  DueDateTo: "",
  SortBy: "Id",
  SortDirection: "Asc"
};

function ensureUtcDate(value: string) {
  const hasTimeZone = /(?:z|[+-]\d{2}:\d{2})$/i.test(value);
  return new Date(hasTimeZone ? value : `${value}Z`);
}

function formatDateTime(value: string | null) {
  if (!value) {
    return "Sin fecha";
  }

  return ensureUtcDate(value).toLocaleString("es-CO");
}

function formatDateOnly(value: string | null) {
  if (!value) {
    return "Sin fecha";
  }

  const [datePart] = value.split("T");
  const [year, month, day] = datePart.split("-");

  if (!year || !month || !day) {
    return formatDateTime(value);
  }

  return `${day}/${month}/${year}`;
}

function mapTaskFormValues(values: TaskFormValues) {
  return {
    Title: values.Title.trim(),
    Description: values.Description.trim() ? values.Description.trim() : null,
    DueDate: values.DueDate ? new Date(`${values.DueDate}T00:00:00`).toISOString() : null,
    StateId: Number(values.StateId)
  };
}

export function TasksPage() {
  const token = useAppSelector(selectToken);
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [states, setStates] = useState<TaskState[]>([]);
  const [filters, setFilters] = useState<TaskFilters>(initialFilters);
  const [appliedFilters, setAppliedFilters] = useState<TaskFilters>(initialFilters);
  const [selectedTask, setSelectedTask] = useState<TaskItem | null>(null);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(5);
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [feedbackMessage, setFeedbackMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isLoadingStates, setIsLoadingStates] = useState(true);
  const [taskToDelete, setTaskToDelete] = useState<TaskItem | null>(null);
  const [taskToView, setTaskToView] = useState<TaskItem | null>(null);

  async function loadStates() {
    if (!token) {
      return;
    }

    setIsLoadingStates(true);

    try {
      const response = await getTaskStates(token);
      setStates(response);
    } catch (error) {
      setErrorMessage(
        error instanceof Error ? error.message : "No fue posible cargar los estados disponibles."
      );
    } finally {
      setIsLoadingStates(false);
    }
  }

  async function loadTasks() {
    if (!token) {
      return;
    }

    setIsLoading(true);
    setErrorMessage(null);

    try {
      const response = await getTasks({
        token,
        pageNumber,
        pageSize,
        search: appliedFilters.Search.trim() || undefined,
        stateId: appliedFilters.StateId ? Number(appliedFilters.StateId) : undefined,
        dueDateFrom: appliedFilters.DueDateFrom || undefined,
        dueDateTo: appliedFilters.DueDateTo || undefined,
        sortBy: appliedFilters.SortBy,
        sortDirection: appliedFilters.SortDirection
      });

      setTasks(response.Items);
      setTotalPages(response.TotalPages);
      setTotalCount(response.TotalCount);
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "No fue posible cargar las tareas.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    void loadStates();
  }, [token]);

  useEffect(() => {
    void loadTasks();
  }, [token, pageNumber, appliedFilters]);

  useEffect(() => {
    if (!feedbackMessage) {
      return;
    }

    const timeout = window.setTimeout(() => setFeedbackMessage(null), 4000);
    return () => window.clearTimeout(timeout);
  }, [feedbackMessage]);

  async function refreshTasks() {
    await loadTasks();
  }

  async function handleFilterSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setErrorMessage(null);

    if (filters.DueDateFrom && filters.DueDateTo && filters.DueDateFrom > filters.DueDateTo) {
      setErrorMessage("La fecha inicial no puede ser mayor que la fecha final.");
      return;
    }

    setAppliedFilters(filters);
    setPageNumber(1);
  }

  function handleFilterChange(
    event: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) {
    const { name, value } = event.target;
    setFilters((current) => ({
      ...current,
      [name]: value
    }));
  }

  function clearFilters() {
    setFilters(initialFilters);
    setAppliedFilters(initialFilters);
    setPageNumber(1);
    setSelectedTask(null);
    setFeedbackMessage(null);
    setErrorMessage(null);
  }

  async function handleSaveTask(values: TaskFormValues) {
    if (!token) {
      return;
    }

    setIsSaving(true);
    setErrorMessage(null);
    setFeedbackMessage(null);

    try {
      const payload = mapTaskFormValues(values);

      if (selectedTask) {
        await updateTask(token, selectedTask.Id, payload);
        setFeedbackMessage("La tarea se actualizó correctamente.");
      } else {
        await createTask(token, payload);
        setFeedbackMessage("La tarea se creó correctamente.");
      }

      setSelectedTask(null);
      await refreshTasks();
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "No fue posible guardar la tarea.");
      throw error;
    } finally {
      setIsSaving(false);
    }
  }

  async function handleDeleteTask() {
    if (!token) {
      return;
    }

    if (!taskToDelete) {
      return;
    }

    setErrorMessage(null);
    setFeedbackMessage(null);

    try {
      await deleteTask(token, taskToDelete.Id);
      if (selectedTask?.Id === taskToDelete.Id) {
        setSelectedTask(null);
      }

      setFeedbackMessage("La tarea se eliminó correctamente.");
      setTaskToDelete(null);
      await refreshTasks();
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "No fue posible eliminar la tarea.");
    }
  }

  return (
    <div className="tasks-layout">
      <section className="surface-card task-list-card">
        <div className="section-heading section-heading--row">
          <div>
            <span className="eyebrow">Módulo de tareas</span>
            <h2>Gestión de tareas</h2>
            <p>Consulta, filtra y administra las tareas registradas en la API.</p>
          </div>

          <div className="metrics-pill">
            <strong>{totalCount}</strong>
            <span>registros</span>
          </div>
        </div>

        <form className="filters-grid" onSubmit={handleFilterSubmit}>
          <label className="field">
            <span>Buscar por título o descripción</span>
            <input
              name="Search"
              type="text"
              value={filters.Search}
              onChange={handleFilterChange}
              placeholder="Ejemplo: informe"
            />
          </label>

          <label className="field">
            <span>Estado</span>
            <select name="StateId" value={filters.StateId} onChange={handleFilterChange}>
              <option value="">Todos los estados</option>
              {states.map((state) => (
                <option key={state.Id} value={state.Id}>
                  {state.Name}
                </option>
              ))}
            </select>
          </label>

          <label className="field">
            <span>Fecha desde</span>
            <input
              name="DueDateFrom"
              type="date"
              value={filters.DueDateFrom}
              onChange={handleFilterChange}
            />
          </label>

          <label className="field">
            <span>Fecha hasta</span>
            <input
              name="DueDateTo"
              type="date"
              value={filters.DueDateTo}
              onChange={handleFilterChange}
            />
          </label>

          <label className="field">
            <span>Ordenar por</span>
            <select name="SortBy" value={filters.SortBy} onChange={handleFilterChange}>
              <option value="Id">Id</option>
              <option value="Title">Título</option>
              <option value="DueDate">Fecha límite</option>
              <option value="StateName">Estado</option>
              <option value="CreatedAt">Fecha de creación</option>
              <option value="UpdatedAt">Última actualización</option>
            </select>
          </label>

          <label className="field">
            <span>Dirección</span>
            <select
              name="SortDirection"
              value={filters.SortDirection}
              onChange={handleFilterChange}
            >
              <option value="Asc">Ascendente</option>
              <option value="Desc">Descendente</option>
            </select>
          </label>

          <div className="filters-actions">
            <button type="button" className="ghost-button" onClick={clearFilters}>
              Limpiar
            </button>
            <button type="submit" className="primary-button">
              Aplicar filtros
            </button>
          </div>
        </form>

        {feedbackMessage ? (
          <FeedbackMessage
            type="success"
            message={feedbackMessage}
            onClose={() => setFeedbackMessage(null)}
          />
        ) : null}
        {errorMessage ? (
          <FeedbackMessage
            type="error"
            message={errorMessage}
            onClose={() => setErrorMessage(null)}
          />
        ) : null}

        {isLoading ? (
          <div className="tasks-empty-state">
            <div className="loader-block" />
            <div className="loader-block loader-block--short" />
            <div className="loader-block" />
          </div>
        ) : tasks.length === 0 ? (
          <div className="tasks-empty-state">
            <h3>No hay tareas para los filtros aplicados</h3>
            <p>Ajusta la búsqueda o crea una nueva tarea desde el formulario lateral.</p>
          </div>
        ) : (
          <div className="data-table tasks-table">
            {tasks.map((task) => (
              <article key={task.Id} className="data-row task-row">
                <div className="task-main">
                  <span className="task-chip">{task.StateName}</span>
                  <h3>{task.Title}</h3>
                  <p>{task.Description || "Sin descripción registrada."}</p>
                </div>

                <dl className="task-table-meta task-table-meta--compact">
                  <div>
                    <dt>Fecha límite</dt>
                    <dd>{formatDateOnly(task.DueDate)}</dd>
                  </div>
                </dl>

                <div className="table-actions">
                  <button
                    type="button"
                    className="ghost-button ghost-button--small"
                    onClick={() => setTaskToView(task)}
                  >
                    Ver detalle
                  </button>
                  <button
                    type="button"
                    className="ghost-button ghost-button--small"
                    onClick={() => setSelectedTask(task)}
                  >
                    Editar
                  </button>
                  <button
                    type="button"
                    className="danger-button"
                    onClick={() => setTaskToDelete(task)}
                  >
                    Eliminar
                  </button>
                </div>
              </article>
            ))}
          </div>
        )}

        <div className="pagination-bar">
          <button
            type="button"
            className="ghost-button"
            disabled={pageNumber <= 1}
            onClick={() => setPageNumber((current) => current - 1)}
          >
            Anterior
          </button>

          <span>
            Página {Math.min(pageNumber, Math.max(totalPages, 1))} de {Math.max(totalPages, 1)}
          </span>

          <button
            type="button"
            className="ghost-button"
            disabled={pageNumber >= totalPages || totalPages === 0}
            onClick={() => setPageNumber((current) => current + 1)}
          >
            Siguiente
          </button>
        </div>
      </section>

      <TaskForm
        states={states}
        selectedTask={selectedTask}
        isSaving={isSaving}
        isStatesReady={!isLoadingStates && states.length > 0}
        onSubmit={handleSaveTask}
        onCancelEdit={() => setSelectedTask(null)}
      />

      {taskToDelete ? (
        <ConfirmDialog
          title="Eliminar tarea"
          message={`¿Deseas eliminar la tarea "${taskToDelete.Title}"?`}
          confirmLabel="Eliminar"
          onConfirm={() => void handleDeleteTask()}
          onCancel={() => setTaskToDelete(null)}
        />
      ) : null}

      {taskToView ? (
        <TaskDetailDialog
          title={taskToView.Title}
          stateName={taskToView.StateName}
          description={taskToView.Description || "Sin descripción registrada."}
          dueDate={formatDateOnly(taskToView.DueDate)}
          createdAt={formatDateTime(taskToView.CreatedAt)}
          updatedAt={formatDateTime(taskToView.UpdatedAt)}
          onClose={() => setTaskToView(null)}
        />
      ) : null}
    </div>
  );
}
