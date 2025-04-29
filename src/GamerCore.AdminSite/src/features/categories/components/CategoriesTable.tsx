import { useModal } from "../../../contexts/ModalContext";
import { useToast } from "../../../contexts/ToastContext";
import { Category } from "../../../types";
import useDeleteCategory from "../hooks/useDeleteCategory";

interface CategoriesTableProps {
  categories: Category[];
  onCategoryDeleted?: (categoryId: number) => void;
}

function CategoriesTable({ categories, onCategoryDeleted }: CategoriesTableProps) {
  const { showModal } = useModal();
  const { addToast } = useToast();

  const { deleteCategory, deleting, error: deleteError } = useDeleteCategory();

  const handleDelete = (category: Category) => {
    showModal({
      type: "danger",
      title: "Delete Product",
      message: `Are you sure you want to delete category "${category.name}"? This action cannot be undone.`,
      confirmText: "Confirm",
      cancelText: "Cancel",
      onConfirm: async () => {
        try {
          await deleteCategory(category.categoryId);

          // Notify the categories page to update
          if (onCategoryDeleted) {
            onCategoryDeleted(category.categoryId);
          }

          addToast({
            type: "success",
            message: `Category ${category.name} was deleted successfully.`,
            autoDismiss: true,
            dismissDelay: 5000
          });
        } catch {
          addToast({
            type: "error",
            message: deleteError || "Failed to delete category.",
            autoDismiss: true,
            dismissDelay: 7500
          })
        }
      }
    })
  };

  return (
    <div className="table-responsive">
      <table className="table table-striped table-hover">
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Description</th>
            <th>Products</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {categories.map((category) => (
            <tr key={category.categoryId}>
              <td>{category.categoryId}</td>
              <td>{category.name}</td>
              <td>{category.description}</td>
              <td>{category.productCount}</td>
              <td>
                <div className="btn-group btn-group-sm float-end">
                  <button className="btn btn-outline-primary d-flex flex-row align-items-center">
                    <i className="bi bi-pencil me-2"></i>
                    Edit
                  </button>
                  <button
                    className="btn btn-outline-danger d-flex flex-row align-items-center"
                    onClick={() => handleDelete(category)}
                    disabled={deleting}
                  >
                    <i className="bi bi-trash me-2"></i>
                    Delete
                  </button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default CategoriesTable;