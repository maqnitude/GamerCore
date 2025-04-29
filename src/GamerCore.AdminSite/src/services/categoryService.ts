import { Category, CreateCategoryPayload } from "../types";

const baseApiEndpoint = "/api/categories";

const CategoryService = {
  getCategories: async (): Promise<Category[]> => {
    const apiEndpoint = baseApiEndpoint;

    const response = await fetch(apiEndpoint, {
      method: "GET"
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    return response.json();
  },

  createCategory: async (payload: CreateCategoryPayload): Promise<number> => {
    const apiEndpoint = baseApiEndpoint;

    const response = await fetch(apiEndpoint, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    })

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    const createdCategoryId = await response.json();
    return createdCategoryId as number;
  }
}

export default CategoryService;