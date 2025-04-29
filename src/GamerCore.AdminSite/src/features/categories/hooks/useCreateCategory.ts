import { useCallback, useState } from "react";
import { Category, CreateCategoryPayload } from "../../../types";
import CategoryService from "../../../services/categoryService";

function useCreateCategory() {
  const [createdCategory, setCreatedCategory] = useState<Category | null>(null);
  const [creating, setCreating] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const createCategory = useCallback(async (payload: CreateCategoryPayload) => {
    try {
      setCreating(true);
      const data = await CategoryService.createCategory(payload);
      setCreatedCategory(data);
      setError(null);

      return data;
    } catch (err) {
      console.error("Error creating category:", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
      throw err;
    } finally {
      setCreating(false);
    }
  }, []);

  return { createCategory, createdCategory, creating, error };
}

export default useCreateCategory;