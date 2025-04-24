import { useEffect, useState } from "react";
import { Category } from "../../../types";
import CategoryService from "../../../services/categoryService";

function useCategories() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchCategories = async () => {
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
    }

    fetchCategories();
  }, []);

  return { categories, loading, error };
}

export default useCategories;