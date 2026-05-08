type FeedbackMessageProps = {
  type: "success" | "error" | "info";
  message: string;
  onClose?: () => void;
};

export function FeedbackMessage({ type, message, onClose }: FeedbackMessageProps) {
  const className =
    type === "success"
      ? "alert alert--success"
      : type === "error"
        ? "alert alert--error"
        : "alert alert--info";

  return (
    <div className={className} role="status" aria-live="polite">
      <span>{message}</span>
      {onClose ? (
        <button type="button" className="alert__close" onClick={onClose} aria-label="Cerrar mensaje">
          x
        </button>
      ) : null}
    </div>
  );
}
