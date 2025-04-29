import { useCallback, useEffect, useState } from "react";
import { Category } from "../../../types";
import CategoryService from "../../../services/categoryService";

function useCategories() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetchCategories = useCallback(async () => {
    try {
      setLoading(true);
      const data = await CategoryService.getCategories();
      setCategories(data);
      setError(null);
    } catch (err) {
      console.error("Error fetching categories: ", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchCategories();
  }, [fetchCategories]);

  return { categories, loading, error };
}

export default useCategories;