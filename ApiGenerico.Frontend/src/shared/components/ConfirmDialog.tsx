type ConfirmDialogProps = {
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  isLoading?: boolean;
  onConfirm: () => void;
  onCancel: () => void;
};

export function ConfirmDialog({
  title,
  message,
  confirmLabel = "Aceptar",
  cancelLabel = "Cancelar",
  isLoading = false,
  onConfirm,
  onCancel
}: ConfirmDialogProps) {
  return (
    <div className="modal-backdrop" role="presentation">
      <section
        className="confirm-dialog"
        role="dialog"
        aria-modal="true"
        aria-labelledby="confirm-dialog-title"
      >
        <div className="confirm-dialog__icon" aria-hidden="true">
          !
        </div>

        <div className="confirm-dialog__copy">
          <h2 id="confirm-dialog-title">{title}</h2>
          <p>{message}</p>
        </div>

        <div className="confirm-dialog__actions">
          <button type="button" className="ghost-button" onClick={onCancel} disabled={isLoading}>
            {cancelLabel}
          </button>
          <button type="button" className="danger-button danger-button--solid" onClick={onConfirm} disabled={isLoading}>
            {isLoading ? "Procesando..." : confirmLabel}
          </button>
        </div>
      </section>
    </div>
  );
}
