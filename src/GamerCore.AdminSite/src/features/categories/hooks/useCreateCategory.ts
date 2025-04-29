import { useCallback, useState } from "react";
import { CreateCategoryPayload } from "../../../types";
import CategoryService from "../../../services/categoryService";

function useCreateCategory() {
  const [createdCategoryId, setCreatedCategoryId] = useState<number | null>(null);
  const [creating, setCreating] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const createCategory = useCallback(async (payload: CreateCategoryPayload) => {
    try {
      setCreating(true);
      const id = await CategoryService.createCategory(payload);
      setCreatedCategoryId(id);
      setError(null);

      return id;
    } catch (err) {
      console.error("Error creating category:", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
      throw err;
    } finally {
      setCreating(false);
    }
  }, []);

  return { createCategory, createdCategoryId, creating, error };
}

export default useCreateCategory;