import { ReactNode, useState } from "react";
import { ToastContext, ToastData } from "./ToastContext";

interface ToastProviderProps {
  children: ReactNode;
}

function ToastProvider({ children }: ToastProviderProps) {
  const [toasts, setToasts] = useState<ToastData[]>([]);

  const addToast = (toast: Omit<ToastData, "id">): number => {
    const id = Date.now();
    setToasts(prev => [...prev, { ...toast, id }]);
    return id;
  }

  const removeToast = (id: number) => {
    setToasts(prev => prev.filter(toast => toast.id !== id));
  }

  const clearAllToasts = () => {
    setToasts([]);
  }

  return (
    <ToastContext.Provider value={{ toasts, addToast, removeToast, clearAllToasts }}>
      {children}
    </ToastContext.Provider>
  );
}

export default ToastProvider;