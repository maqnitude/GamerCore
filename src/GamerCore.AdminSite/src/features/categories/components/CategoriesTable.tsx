import { useModal } from "../../../contexts/ModalContext";
import { useToast } from "../../../contexts/ToastContext";
import { Category } from "../../../types";
import { formatDate } from "../../../utils";
import useDeleteCategory from "../hooks/useDeleteCategory";

interface CategoriesTableProps {
  categories: Category[];
  onEditButtonClick: (categoryId: string) => void;
  onCategoryDeleted?: (categoryId: string) => void;
}

function CategoriesTable({
  categories,
  onEditButtonClick,
  onCategoryDeleted,
}: CategoriesTableProps) {
  const { showModal } = useModal();
  const { addToast } = useToast();

  const { deleteCategory, deleting, error: deleteError } = useDeleteCategory();

  const handleEdit = (categoryId: string) => {
    // Tell categories page to open the update form
    onEditButtonClick(categoryId);
  };

  const handleDelete = (category: Category) => {
    showModal({
      type: "danger",
      title: "Delete Product",
      message: `Are you sure you want to delete category "${category.name}"? This action cannot be undone.`,
      confirmText: "Confirm",
      cancelText: "Cancel",
      onConfirm: async () => {
        try {
          await deleteCategory(category.id);

          // Notify the categories page to update
          if (onCategoryDeleted) {
            onCategoryDeleted(category.id);
          }

          addToast({
            type: "success",
            message: `Category ${category.name} was deleted successfully.`,
            autoDismiss: true,
            dismissDelay: 5000,
          });
        } catch {
          addToast({
            type: "error",
            message: deleteError || "Failed to delete category.",
            autoDismiss: true,
            dismissDelay: 7500,
          });
        }
      },
    });
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
            <th>Created (UTC)</th>
            <th>Updated (UTC)</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {categories.map((category) => (
            <tr key={category.id}>
              <td>{category.id}</td>
              <td>{category.name}</td>
              <td>{category.description}</td>
              <td>{category.productCount}</td>
              <td>{formatDate(category.createdAt)}</td>
              {/**
               * BUG: when the local category list re-render this displays as local time
               * instead of UTC until the page is refreshed
               * */}
              <td>{formatDate(category.updatedAt)}</td>
              <td>
                <div className="btn-group btn-group-sm float-end">
                  <button
                    className="btn btn-outline-primary d-flex flex-row align-items-center"
                    onClick={() => handleEdit(category.id)}
                  >
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
