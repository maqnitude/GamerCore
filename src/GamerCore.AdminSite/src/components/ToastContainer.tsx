import { useEffect, useState } from "react";
import { createPortal } from "react-dom";
import { useToast } from "../contexts/ToastContext";
import Toast from "./Toast";

function ToastContainer() {
  const { toasts, removeToast } = useToast();
  const [container, setContainer] = useState<HTMLElement | null>(null);

  useEffect(() => {
    // Find or create toast container
    let containerElement = document.getElementById("toastContainer");
    let created = false;

    if (!containerElement) {
      containerElement = document.createElement("div");
      containerElement.id = "toastContainer";
      containerElement.className = "toast-container position-fixed bottom-0 end-0 p-2";

      document.body.appendChild(containerElement);
      created = true;
    }

    setContainer(containerElement);

    return () => {
      if (created && containerElement && containerElement.parentNode) {
        containerElement.parentNode.removeChild(containerElement);
      }
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