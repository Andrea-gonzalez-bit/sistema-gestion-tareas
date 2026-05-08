import { useEffect } from "react";
import { useForm } from "react-hook-form";
import type { StateFormValues, StateItem } from "./stateTypes";

type StateFormProps = {
  selectedState: StateItem | null;
  isSaving: boolean;
  onSubmit: (values: StateFormValues) => Promise<void>;
  onCancelEdit: () => void;
};

export function StateForm({
  selectedState,
  isSaving,
  onSubmit,
  onCancelEdit
}: StateFormProps) {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm<StateFormValues>({
    defaultValues: {
      Name: ""
    }
  });

  useEffect(() => {
    reset({
      Name: selectedState?.Name ?? ""
    });
  }, [reset, selectedState]);

  const submitForm = handleSubmit(async (values) => {
    await onSubmit(values);
    reset({
      Name: ""
    });
  });

  return (
    <section className="surface-card task-form-card">
      <div className="section-heading">
        <span className="eyebrow">{selectedState ? "Edicion" : "Registro"}</span>
        <h2>{selectedState ? "Actualizar estado" : "Crear nuevo estado"}</h2>
        <p>
          {selectedState
            ? "Ajusta el nombre del estado seleccionado."
            : "Crea un estado reusable para la gestión de tareas."}
        </p>
      </div>

      <form className="task-form" onSubmit={submitForm}>
        <label className="field">
          <span>Nombre del estado</span>
          <input
            type="text"
            placeholder="Ejemplo: Pendiente"
            {...register("Name", {
              required: "El nombre es obligatorio.",
              maxLength: {
                value: 100,
                message: "El nombre no puede superar 100 caracteres."
              }
            })}
          />
          {errors.Name ? <small>{errors.Name.message}</small> : null}
        </label>

        <div className="task-form-actions">
          {selectedState ? (
            <button type="button" className="ghost-button" onClick={onCancelEdit}>
              Cancelar edicion
            </button>
          ) : null}

          <button type="submit" className="primary-button" disabled={isSaving}>
            {isSaving
              ? "Guardando..."
              : selectedState
                ? "Guardar cambios"
                : "Crear estado"}
          </button>
        </div>
      </form>
    </section>
  );
}
