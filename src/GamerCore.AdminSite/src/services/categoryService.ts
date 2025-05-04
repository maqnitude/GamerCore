import { Category, CreateCategoryPayload, UpdateCategoryPayload } from "../types";
import authService from "./authService";

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

  getCategory: async (id: number): Promise<Category> => {
    const apiEndpoint = baseApiEndpoint + `/${id}`;

    const response = await fetch(apiEndpoint, {
      method: "GET"
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    return response.json();
  },

  createCategory: async (payload: CreateCategoryPayload): Promise<Category> => {
    const apiEndpoint = baseApiEndpoint;

    const token = authService.getToken();
    if (!token) {
      throw new Error("Not authenticated");
    }

    const response = await fetch(apiEndpoint, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      },
      body: JSON.stringify(payload)
    })

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    return response.json();
  },

  updateCategory: async (id: number, payload: UpdateCategoryPayload): Promise<Category> => {
    const apiEndpoint = baseApiEndpoint + `/${id}`;

    const token = authService.getToken();
    if (!token) {
      throw new Error("Not authenticated");
    }

    const response = await fetch(apiEndpoint, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      },
      body: JSON.stringify(payload)
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    return response.json();
  },

  deleteCategory: async (id: number): Promise<void> => {
    const apiEndpoint = baseApiEndpoint + `/${id}`;

    const token = authService.getToken();
    if (!token) {
      throw new Error("Not authenticated");
    }

    const response = await fetch(apiEndpoint, {
      method: "DELETE",
      headers: {
        "Authorization": `Bearer ${token}`
      }
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }
  }
}

export default CategoryService;