import { useCallback, useState } from "react";
import ProductService from "../../../services/productService";

function useDeleteProduct() {
  const [deleting, setDeleting] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const deleteProduct = useCallback(async (id: string) => {
    try {
      setDeleting(true);
      await ProductService.deleteProduct(id);
      setError(null);
    } catch (err) {
      console.error("Error deleting product:", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
      throw err;
    } finally {
      setDeleting(false);
    }
  }, []);

  return { deleteProduct, deleting, error };
}

export default useDeleteProduct;