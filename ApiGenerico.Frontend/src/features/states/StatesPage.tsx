import { useEffect, useState } from "react";
import { useAppSelector } from "../../hooks";
import { selectToken } from "../auth/authSlice";
import { createState, deleteState, getStates, updateState } from "./stateApi";
import { StateForm } from "./StateForm";
import type { StateFormValues, StateItem } from "./stateTypes";
import { FeedbackMessage } from "../../shared/components/FeedbackMessage";
import { ConfirmDialog } from "../../shared/components/ConfirmDialog";
import { StateDetailDialog } from "../../shared/components/StateDetailDialog";

type StateSortBy = "Id" | "Name" | "TasksCount" | "CreatedAt" | "UpdatedAt";
type SortDirection = "Asc" | "Desc";

function ensureUtcDate(value: string) {
  const hasTimeZone = /(?:z|[+-]\d{2}:\d{2})$/i.test(value);
  return new Date(hasTimeZone ? value : `${value}Z`);
}

function formatDateTime(value: string) {
  return ensureUtcDate(value).toLocaleString("es-CO");
}

function mapStateFormValues(values: StateFormValues) {
  return {
    Name: values.Name.trim()
  };
}

function compareStates(a: StateItem, b: StateItem, sortBy: StateSortBy) {
  switch (sortBy) {
    case "Name":
      return a.Name.localeCompare(b.Name, "es");
    case "TasksCount":
      return a.TasksCount - b.TasksCount;
    case "CreatedAt":
      return ensureUtcDate(a.CreatedAt).getTime() - ensureUtcDate(b.CreatedAt).getTime();
    case "UpdatedAt":
      return ensureUtcDate(a.UpdatedAt).getTime() - ensureUtcDate(b.UpdatedAt).getTime();
    case "Id":
    default:
      return a.Id - b.Id;
  }
}

export function StatesPage() {
  const token = useAppSelector(selectToken);
  const [states, setStates] = useState<StateItem[]>([]);
  const [selectedState, setSelectedState] = useState<StateItem | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [feedbackMessage, setFeedbackMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [stateToDelete, setStateToDelete] = useState<StateItem | null>(null);
  const [stateToView, setStateToView] = useState<StateItem | null>(null);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(5);
  const [sortBy, setSortBy] = useState<StateSortBy>("Id");
  const [sortDirection, setSortDirection] = useState<SortDirection>("Asc");

  const totalPages = Math.ceil(states.length / pageSize);
  const sortedStates = [...states].sort((a, b) => {
    const result = compareStates(a, b, sortBy);
    return sortDirection === "Desc" ? result * -1 : result;
  });
  const pagedStates = sortedStates.slice((pageNumber - 1) * pageSize, pageNumber * pageSize);

  async function loadStates() {
    if (!token) {
      return;
    }

    setIsLoading(true);
    setErrorMessage(null);

    try {
      const response = await getStates(token);
      setStates(response);
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "No fue posible cargar los estados.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    void loadStates();
  }, [token]);

  useEffect(() => {
    if (pageNumber > Math.max(totalPages, 1)) {
      setPageNumber(Math.max(totalPages, 1));
    }
  }, [pageNumber, totalPages]);

  useEffect(() => {
    if (!feedbackMessage) {
      return;
    }

    const timeout = window.setTimeout(() => setFeedbackMessage(null), 4000);
    return () => window.clearTimeout(timeout);
  }, [feedbackMessage]);

  async function handleSaveState(values: StateFormValues) {
    if (!token) {
      return;
    }

    setIsSaving(true);
    setErrorMessage(null);
    setFeedbackMessage(null);

    try {
      const payload = mapStateFormValues(values);

      if (selectedState) {
        await updateState(token, selectedState.Id, payload);
        setFeedbackMessage("El estado se actualizó correctamente.");
      } else {
        await createState(token, payload);
        setFeedbackMessage("El estado se creó correctamente.");
      }

      setSelectedState(null);
      await loadStates();
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : "No fue posible guardar el estado.");
      throw error;
    } finally {
      setIsSaving(false);
    }
  }

  async function handleDeleteState() {
    if (!token) {
      return;
    }

    if (!stateToDelete) {
      return;
    }

    setErrorMessage(null);
    setFeedbackMessage(null);

    try {
      await deleteState(token, stateToDelete.Id);

      if (selectedState?.Id === stateToDelete.Id) {
        setSelectedState(null);
      }

      setFeedbackMessage("El estado se eliminó correctamente.");
      setStateToDelete(null);
      setStateToView(null);
      await loadStates();
    } catch (error) {
      setStateToDelete(null);
      setErrorMessage(error instanceof Error ? error.message : "No fue posible eliminar el estado.");
    }
  }

  return (
    <div className="tasks-layout">
      <section className="surface-card task-list-card">
        <div className="section-heading section-heading--row">
          <div>
            <span className="eyebrow">Módulo de estados</span>
            <h2>Gestión de estados</h2>
            <p>Administra el catálogo de estados que utiliza el flujo de tareas.</p>
          </div>

          <div className="metrics-pill">
            <strong>{states.length}</strong>
            <span>estados</span>
          </div>
        </div>

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
        ) : states.length === 0 ? (
          <div className="tasks-empty-state">
            <h3>No hay estados registrados</h3>
            <p>Crea el primer estado desde el formulario lateral.</p>
          </div>
        ) : (
          <div className="data-table states-table">
            {pagedStates.map((state) => (
              <article key={state.Id} className="data-row state-row">
                <div className="state-main">
                  <span className="task-chip">Estado #{state.Id}</span>
                  <h3>{state.Name}</h3>
                </div>

                <dl className="state-meta state-meta--compact">
                  <div>
                    <dt>Tareas asociadas</dt>
                    <dd>{state.TasksCount}</dd>
                  </div>
                </dl>

                <div className="table-actions">
                  <button
                    type="button"
                    className="ghost-button ghost-button--small"
                    onClick={() => setStateToView(state)}
                  >
                    Ver detalle
                  </button>
                  <button
                    type="button"
                    className="ghost-button ghost-button--small"
                    onClick={() => setSelectedState(state)}
                  >
                    Editar
                  </button>
                  <button
                    type="button"
                    className="danger-button"
                    onClick={() => setStateToDelete(state)}
                  >
                    Eliminar
                  </button>
                </div>
              </article>
            ))}
          </div>
        )}

        {!isLoading && states.length > 0 ? (
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
        ) : null}
      </section>

      <StateForm
        selectedState={selectedState}
        isSaving={isSaving}
        onSubmit={handleSaveState}
        onCancelEdit={() => setSelectedState(null)}
      />

      {stateToDelete ? (
        <ConfirmDialog
          title="Eliminar estado"
          message={`¿Deseas eliminar el estado "${stateToDelete.Name}"?`}
          confirmLabel="Eliminar"
          onConfirm={() => void handleDeleteState()}
          onCancel={() => setStateToDelete(null)}
        />
      ) : null}

      {stateToView ? (
        <StateDetailDialog
          id={stateToView.Id}
          name={stateToView.Name}
          tasksCount={stateToView.TasksCount}
          createdAt={formatDateTime(stateToView.CreatedAt)}
          updatedAt={formatDateTime(stateToView.UpdatedAt)}
          onClose={() => setStateToView(null)}
        />
      ) : null}
    </div>
  );
}
