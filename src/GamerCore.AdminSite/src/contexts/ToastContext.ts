import { createContext, useContext } from "react";

export type ToastType = "success" | "error" | "info" | "warning"

export interface ToastData {
  id: number;
  type: ToastType;
  message: string;
  metadata?: Record<string, unknown>;
  autoDismiss?: boolean;
  dismissDelay?: number;
}

interface ToastContextType {
  toasts: ToastData[];
  addToast: (toast: Omit<ToastData, "id">) => number;
  removeToast: (id: number) => void;
  clearAllToasts: () => void;
}

export const ToastContext = createContext<ToastContextType | undefined>(undefined);

export function useToast() {
  const context = useContext(ToastContext);
  if (!context) {
    throw new Error("useToast must be within a ToastProvider");
  }
  return context;
}