import { useEffect } from "react";
import { useForm } from "react-hook-form";
import type { TaskFormValues, TaskItem, TaskState } from "./taskTypes";

type TaskFormProps = {
  states: TaskState[];
  selectedTask: TaskItem | null;
  isSaving: boolean;
  isStatesReady: boolean;
  onSubmit: (values: TaskFormValues) => Promise<void>;
  onCancelEdit: () => void;
};

function toInputDate(value: string | null) {
  if (!value) {
    return "";
  }

  return value.slice(0, 10);
}

function getEmptyTaskValues(states: TaskState[]): TaskFormValues {
  return {
    Title: "",
    Description: "",
    DueDate: "",
    StateId: states[0] ? String(states[0].Id) : ""
  };
}

export function TaskForm({
  states,
  selectedTask,
  isSaving,
  isStatesReady,
  onSubmit,
  onCancelEdit
}: TaskFormProps) {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm<TaskFormValues>({
    defaultValues: {
      Title: "",
      Description: "",
      DueDate: "",
      StateId: ""
    }
  });

  useEffect(() => {
    reset({
      Title: selectedTask?.Title ?? "",
      Description: selectedTask?.Description ?? "",
      DueDate: toInputDate(selectedTask?.DueDate ?? null),
      StateId: selectedTask ? String(selectedTask.StateId) : states[0] ? String(states[0].Id) : ""
    });
  }, [reset, selectedTask, states]);

  const submitForm = handleSubmit(async (values) => {
    await onSubmit(values);
    reset(getEmptyTaskValues(states));
  });

  return (
    <section className="surface-card task-form-card">
      <div className="section-heading">
        <span className="eyebrow">{selectedTask ? "Edicion" : "Registro"}</span>
        <h2>{selectedTask ? "Actualizar tarea" : "Crear nueva tarea"}</h2>
        <p>
          {selectedTask
            ? "Modifica la informacion principal de la tarea seleccionada."
            : "Registra una tarea y asóciala al estado correspondiente."}
        </p>
      </div>

      <form className="task-form" onSubmit={submitForm}>
        <label className="field">
          <span>Titulo</span>
          <input
            type="text"
            placeholder="Ejemplo: Preparar informe de ventas"
            {...register("Title", {
              required: "El titulo es obligatorio.",
              maxLength: {
                value: 200,
                message: "El titulo no puede superar 200 caracteres."
              }
            })}
          />
          {errors.Title ? <small>{errors.Title.message}</small> : null}
        </label>

        <label className="field">
          <span>Descripcion</span>
          <textarea
            rows={4}
            placeholder="Agrega detalles opcionales de la tarea"
            {...register("Description", {
              maxLength: {
                value: 2000,
                message: "La descripción no puede superar 2000 caracteres."
              }
            })}
          />
          {errors.Description ? <small>{errors.Description.message}</small> : null}
        </label>

        <div className="task-form-grid">
          <label className="field">
            <span>Fecha límite</span>
            <input type="date" {...register("DueDate")} />
          </label>

          <label className="field">
            <span>Estado</span>
            <select
              disabled={!isStatesReady}
              {...register("StateId", {
                required: "Debes seleccionar un estado."
              })}
            >
              <option value="">Selecciona un estado</option>
              {states.map((state) => (
                <option key={state.Id} value={state.Id}>
                  {state.Name}
                </option>
              ))}
            </select>
            {errors.StateId ? <small>{errors.StateId.message}</small> : null}
          </label>
        </div>

        {!isStatesReady ? (
          <div className="inline-note">
            Los estados se están cargando o no se encontraron disponibles para registrar tareas.
          </div>
        ) : null}

        <div className="task-form-actions">
          {selectedTask ? (
            <button type="button" className="ghost-button" onClick={onCancelEdit}>
              Cancelar edicion
            </button>
          ) : null}

          <button type="submit" className="primary-button" disabled={isSaving}>
            {isSaving
              ? "Guardando..."
              : selectedTask
                ? "Guardar cambios"
                : "Crear tarea"}
          </button>
        </div>
      </form>
    </section>
  );
}
