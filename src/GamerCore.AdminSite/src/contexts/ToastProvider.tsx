import { ReactNode, useEffect, useRef, useState } from "react";
import { ToastContext, ToastData } from "./ToastContext";

interface ToastProviderProps {
  children: ReactNode;
}

function ToastProvider({ children }: ToastProviderProps) {
  const [toasts, setToasts] = useState<ToastData[]>([]);
  const nextIdRef = useRef<number>(0);

  useEffect(() => {
    if (toasts.length === 0) {
      nextIdRef.current = 0;
    }
  }, [toasts]);

  const addToast = (toast: Omit<ToastData, "id">): number => {
    const id = nextIdRef.current++;
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