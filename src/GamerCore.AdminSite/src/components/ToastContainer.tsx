import { useEffect, useState } from "react";
import { createPortal } from "react-dom";
import { ToastData, useToast } from "../contexts/ToastContext";

interface ToastProps {
  toast: ToastData;
  onClose: () => void;
}

function Toast({ toast, onClose }: ToastProps) {
  useEffect(() => {
    if (toast.autoDismiss !== false) {
      const timer = setTimeout(() => {
        onClose();
      }, toast.dismissDelay || 5000);

      return () => clearTimeout(timer);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // Leave deps empty so the timer won't reset

  const getToastStyles = () => {
    switch (toast.type) {
      case "success":
        return {
          headerClass: "bg-success text-white",
          icon: "bi bi-check-circle",
          title: "Success"
        };
      case "error":
        return {
          headerClass: "bg-danger text-white",
          icon: "bi bi-exclamation-triangle",
          title: "Error"
        };
      case "warning":
        return {
          headerClass: "bg-warning text-dark",
          icon: "bi bi-exclamation-circle",
          title: "Warning"
        };
      case "info":
      default:
        return {
          headerClass: "bg-info text-white",
          icon: "bi bi-info-circle",
          title: "Information"
        };
    }
  };

  const styles = getToastStyles();

  return (
    <div
      className="toast show"
      role="alert"
      aria-live="assertive"
      aria-atomic="true"
    >
      <div className={`toast-header ${styles.headerClass}`}>
        <i className={`${styles.icon} me-2`}></i>
        <strong className="me-auto">{styles.title}</strong>
        {toast.type === "success"
          && toast.metadata?.createdProductId as number
          && <small className="text-white">ID: {toast.metadata?.createdProductId as number}</small>}
        <button
          type="button"
          className="btn-close"
          data-bs-dismiss="toast"
          aria-label="Close"
          onClick={onClose}
        ></button>
      </div>
      <div className="toast-body">{toast.message}</div>
    </div>
  );
}

function ToastContainer() {
  const { toasts, removeToast } = useToast();
  const [container, setContainer] = useState<HTMLElement | null>(null);

  useEffect(() => {
    // Find or create toast container
    let containerElement = document.getElementById("toastContainer");

    if (!containerElement) {
      containerElement = document.createElement("div");
      containerElement.id = "toastContainer";
      containerElement.className = "toast-container position-fixed bottom-0 end-0 p-2";

      document.body.appendChild(containerElement);
    }

    setContainer(containerElement);

    return () => {
      document.body.removeChild(containerElement);
    }
  }, []);

  if (!container) {
    return null;
  }

  return createPortal(
    <>
      {toasts.map(toast => (
        <Toast
          key={toast.id}
          toast={toast}
          onClose={() => removeToast(toast.id)}
        />
      ))}
    </>,
    container
  );
}

export default ToastContainer;