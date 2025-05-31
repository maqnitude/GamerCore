import { useCallback, useState } from "react";
import { UpdateCategoryPayload } from "../../../types";
import CategoryService from "../../../services/categoryService";

function useUpdateCategory() {
  const [updating, setUpdating] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const updateCategory = useCallback(async (id: string, payload: UpdateCategoryPayload) => {
    try {
      setUpdating(true);
      const updatedCategory = await CategoryService.updateCategory(id, payload);
      setError(null);

      return updatedCategory;
    } catch (err) {
      console.error("Error creating category:", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
      throw err;
    } finally {
      setUpdating(false);
    }
  }, []);

  return { updateCategory, updating, error };
}

export default useUpdateCategory;