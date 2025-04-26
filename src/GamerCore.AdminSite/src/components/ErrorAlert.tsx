import { useEffect, useState } from "react";
import { createPortal } from "react-dom";

interface ErrorAlertProps {
  alertContainerId?: string,
  message: string
}

function ErrorAlert({
  alertContainerId = "alertContainer",
  message
}: ErrorAlertProps) {
  const [container, setContainer] = useState<HTMLElement | null>(null);

  useEffect(() => {
    const el = document.getElementById(alertContainerId);

    if (!el) {
      console.error("Error alert container", alertContainerId, "not found.");
      return;
    }

    setContainer(el);
  }, [alertContainerId]);

  if (!container) {
    return null;
  }

  return createPortal(
    <div
      className="alert alert-danger alert-dismissible m-3"
      role="alert"
      style={{ whiteSpace: "pre-line" }} // Enables new line with '\n'
    >
      <div>
        <i className="bi bi-exclamation-triangle-fill me-2"></i>
        Error: {message}
      </div>
      <button
        type="button"
        className="btn-close"
        data-bs-dismiss="alert"
        aria-label="Close"
      ></button>
    </div>,
    container
  );
}

export default ErrorAlert;