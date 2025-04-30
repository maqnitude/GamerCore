import { useEffect } from "react";
import { ToastData } from "../contexts/ToastContext";

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

  // IMPORTANT: do not use bootstrap js for Toast, will cause DOMException
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
          && toast.metadata
          && toast.metadata.createdProductId as number
          && <small className="text-white">ID: {toast.metadata.createdProductId as number}</small>}

        {toast.type === "success"
          && toast.metadata
          && toast.metadata.createdCategoryId as number
          && <small className="text-white">ID: {toast.metadata.createdCategoryId as number}</small>}
        <button
          type="button"
          className="btn-close"
          aria-label="Close"
          onClick={onClose}
        ></button>
      </div>
      <div className="toast-body">{toast.message}</div>
    </div>
  );
}

export default Toast;