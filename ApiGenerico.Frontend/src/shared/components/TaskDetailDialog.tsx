type TaskDetailDialogProps = {
  title: string;
  stateName: string;
  description: string;
  dueDate: string;
  createdAt: string;
  updatedAt: string;
  onClose: () => void;
};

export function TaskDetailDialog({
  title,
  stateName,
  description,
  dueDate,
  createdAt,
  updatedAt,
  onClose
}: TaskDetailDialogProps) {
  return (
    <div className="modal-backdrop" role="presentation">
      <section
        className="detail-dialog"
        role="dialog"
        aria-modal="true"
        aria-labelledby="task-detail-title"
      >
        <div className="detail-dialog__header">
          <div>
            <span className="task-chip">{stateName}</span>
            <h2 id="task-detail-title">{title}</h2>
          </div>

          <button type="button" className="alert__close detail-dialog__close" onClick={onClose} aria-label="Cerrar detalle">
            x
          </button>
        </div>

        <div className="detail-dialog__description">
          <span>Descripcion</span>
          <p>{description}</p>
        </div>

        <dl className="detail-grid">
          <div>
            <dt>Fecha límite</dt>
            <dd>{dueDate}</dd>
          </div>
          <div>
            <dt>Creada</dt>
            <dd>{createdAt}</dd>
          </div>
          <div>
            <dt>Actualizada</dt>
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
