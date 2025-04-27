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
  const [visible, setVisible] = useState<boolean>(false);

  useEffect(() => {
    const el = document.getElementById(alertContainerId);

    if (!el) {
      console.error("Error alert container", alertContainerId, "not found.");
      return;
    }

    setContainer(el);
  }, [alertContainerId]);

  // Reset visibility when message changes
  useEffect(() => {
    setVisible(true);
  }, [message]);

  const handleClose = () => {
    setVisible(false);
  };

  if (!container || !visible) {
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
        aria-label="Close"
        onClick={handleClose}
      ></button>
    </div>,
    container
  );
}

export default ErrorAlert;