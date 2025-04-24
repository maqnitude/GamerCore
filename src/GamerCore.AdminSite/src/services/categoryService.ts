import { Category } from "../types";

const CategoryService = {
  getCategories: async (): Promise<Category[]> => {
    const response = await fetch("/api/categories");
    if (!response.ok) {
      throw new Error(`Network error - Status: ${response.status}`);
    }
    return response.json();
  }
}

export default CategoryService;