import { Category } from "../types";

const CategoryService = {
  getCategories: async (): Promise<Category[]> => {
    const response = await fetch("/api/categories");
    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }
    return response.json();
  }
}

export default CategoryService;