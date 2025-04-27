import { createContext, useContext } from "react";

export type ModalType = "info" | "warning" | "danger";

export interface ModalData {
  type: ModalType;
  title?: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  onConfirm: () => void;
  onCancel?: () => void;
}

export interface ModalState {
  isOpen: boolean,
  data: ModalData | null;
}

interface ModalContextType {
  showModal: (data: ModalData) => void;
  hideModal: () => void;
  modalState: ModalState;
}

export const ModalContext = createContext<ModalContextType | undefined>(undefined);

export function useModal() {
  const context = useContext(ModalContext);
  if (!context) {
    throw new Error("useModal must be used within a ModalProvider");
  }
  return context;
}