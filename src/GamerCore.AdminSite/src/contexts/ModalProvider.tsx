import { ReactNode, useState } from "react";
import { ModalContext, ModalData, ModalState } from "./ModalContext";

interface ModalProviderProps {
  children: ReactNode
}

function ModalProvider({ children }: ModalProviderProps) {
  const [modalState, setModalState] = useState<ModalState>({ isOpen: false, data: null });

  const showModal = (data: ModalData) => {
    setModalState({
      isOpen: true,
      data: data
    });
  };

  const hideModal = () => {
    setModalState({
      isOpen: false,
      data: null
    });
  };

  return (
    <ModalContext.Provider value={{ showModal, hideModal, modalState }}>
      {children}
    </ModalContext.Provider>
  );
}

export default ModalProvider;