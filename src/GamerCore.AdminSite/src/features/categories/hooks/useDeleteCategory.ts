import { useCallback, useState } from "react";
import CategoryService from "../../../services/categoryService";

function useDeleteCategory() {
  const [deleting, setDeleting] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const deleteCategory = useCallback(async (id: string) => {
    try {
      setDeleting(true);
      await CategoryService.deleteCategory(id);
      setError(null);
    } catch (err) {
      console.error("Error deleting product:", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
      throw err;
    } finally {
      setDeleting(false);
    }
  }, []);

  return { deleteCategory, deleting, error };
}

export default useDeleteCategory;
