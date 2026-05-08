type StateDetailDialogProps = {
  id: number;
  name: string;
  tasksCount: number;
  createdAt: string;
  updatedAt: string;
  onClose: () => void;
};

export function StateDetailDialog({
  id,
  name,
  tasksCount,
  createdAt,
  updatedAt,
  onClose
}: StateDetailDialogProps) {
  return (
    <div className="modal-backdrop" role="presentation">
      <section
        className="detail-dialog"
        role="dialog"
        aria-modal="true"
        aria-labelledby="state-detail-title"
      >
        <div className="detail-dialog__header">
          <div>
            <span className="task-chip">Estado #{id}</span>
            <h2 id="state-detail-title">{name}</h2>
          </div>

          <button type="button" className="alert__close detail-dialog__close" onClick={onClose} aria-label="Cerrar detalle">
            x
          </button>
        </div>

        <dl className="detail-grid">
          <div>
            <dt>Tareas asociadas</dt>
            <dd>{tasksCount}</dd>
          </div>
          <div>
            <dt>Creado</dt>
            <dd>{createdAt}</dd>
          </div>
          <div>
            <dt>Actualizado</dt>
            <dd>{updatedAt}</dd>
          </div>
        </dl>

        <div className="confirm-dialog__actions">
          <button type="button" className="primary-button" onClick={onClose}>
            Entendido
          </button>
        </div>
      </section>
    </div>
  );
}
