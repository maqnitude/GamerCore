import { useNavigate } from "react-router";
import { useModal } from "../../../contexts/ModalContext";
import { useToast } from "../../../contexts/ToastContext";
import { Product } from "../../../types";
import useDeleteProduct from "../hooks/useDeleteProduct";
import { formatDate } from "../../../utils";

interface ProductsTableProps {
  products: Product[];
  onProductDeleted?: (productId: number) => void;
}

function ProductsTable({ products, onProductDeleted }: ProductsTableProps) {
  const { addToast } = useToast();
  const { showModal } = useModal();

  const { deleteProduct, deleting, error: deleteError } = useDeleteProduct();

  const navigate = useNavigate();

  const handleViewDetails = (productId: number) => {
    navigate(`/products/details/${productId}`);
  };

  const handleUpdate = (productId: number) => {
    navigate(`/products/edit/${productId}`);
  };

  const handleDelete = (product: Product) => {
    showModal({
      type: "danger",
      title: "Delete Product",
      message: `Are you sure you want to delete product "${product.name}"? This action cannot be undone.`,
      confirmText: "Confirm",
      cancelText: "Cancel",
      onConfirm: async () => {
        try {
          await deleteProduct(product.productId);

          // Notify the products page to update
          if (onProductDeleted) {
            onProductDeleted(product.productId);
          }

          addToast({
            type: "success",
            message: `Product ${product.name} was deleted successfully.`,
            autoDismiss: true,
            dismissDelay: 5000
          });
        } catch {
          addToast({
            type: "error",
            message: deleteError || "Failed to delete product.",
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
            <th>Price</th>
            <th>Rating</th>
            <th>Reviews</th>
            <th>Created (UTC)</th>
            <th>Updated (UTC)</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {products.map((product) => (
            <tr key={product.productId}>
              <td>{product.productId}</td>
              <td>{product.name}</td>
              <td>{product.price.toFixed(2)}</td>
              <td>
                <div className="d-flex align-items-center">
                  <i className="bi bi-star-fill text-warning"></i>
                  <span className="ms-1">{product.averageRating.toFixed(2)}</span>
                </div>
              </td>
              <td>{product.reviewCount}</td>
              <td>{formatDate(product.createdAt)}</td>
              <td>{formatDate(product.updatedAt)}</td>
              <td>
                <div className="btn-group btn-group-sm float-end">
                  <button
                    className="btn btn-outline-info"
                    onClick={() => handleViewDetails(product.productId)}
                  >
                    <i className="bi bi-eye me-2"></i>
                    Details
                  </button>
                  <button
                    className="btn btn-outline-primary"
                    onClick={() => handleUpdate(product.productId)}
                  >
                    <i className="bi bi-pencil me-2"></i>
                    Edit
                  </button>
                  <button
                    className="btn btn-outline-danger"
                    onClick={() => handleDelete(product)}
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

export default ProductsTable;