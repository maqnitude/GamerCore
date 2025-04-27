import { useModal } from "../contexts/ModalContext";

function Modal() {
  const { modalState, hideModal } = useModal();
  const { isOpen, data } = modalState;

  if (!isOpen || !data) {
    return null;
  }

  const getHeaderClass = () => {
    switch (data.type) {
      case "danger":
        return "bg-danger text-white";
      case "warning":
        return "bg-warning";
      case "info":
        return "bg-info text-white";
      default:
        return "bg-primary text-white";
    }
  };

  const getConfirmButtonClass = () => {
    switch (data.type) {
      case "danger":
        return "btn-danger";
      case "warning":
        return "btn-warning";
      case "info":
        return "btn-info";
      default:
        return "btn-primary";
    }
  };

  const handleConfirm = () => {
    data.onConfirm();
    hideModal();
  }

  const handleCancel = () => {
    if (data.onCancel) {
      data.onCancel();
    }
    hideModal();
  }

  return (
    <>
      {/* Display block to force the modal to display a block element */}
      <div className="modal fade show d-block" tabIndex={-1} role="dialog">
        <div className="modal-dialog modal-dialog-centered" role="document">
          <div className="modal-content">
            <div className={`modal-header ${getHeaderClass()}`}>
              <h5 className="modal-title">{data.title || "Confirm Action"}</h5>
              <button
                type="button"
                className="btn-close"
                onClick={handleCancel}
                aria-label="Close"
              ></button>
            </div>
            <div className="modal-body">
              <p>{data.message}</p>
            </div>
            <div className="modal-footer">
              <button
                type="button"
                className="btn btn-secondary"
                onClick={handleCancel}
              >
                {data.cancelText || "Cancel"}
              </button>
              <button
                type="button"
                className={`btn ${getConfirmButtonClass()}`}
                onClick={handleConfirm}
              >
                {data.confirmText || "Confirm"}
              </button>
            </div>
          </div>
        </div>
      </div>
      <div className="modal-backdrop show"></div>
    </>
  );
}

export default Modal;