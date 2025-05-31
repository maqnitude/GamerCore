import {
  CreateProductPayload,
  PaginatedList,
  Product,
  ProductDetails,
  UpdateProductPayload,
} from "../types";
import authService from "./authService";

const baseApiEndpoint = "/api/products";

const ProductService = {
  getProducts: async (
    page: number = 1,
    categoryIds: string[] | null
  ): Promise<PaginatedList<Product>> => {
    let apiEndpoint = baseApiEndpoint;

    apiEndpoint += `?page=${page}`;

    if (categoryIds) {
      categoryIds.forEach((categoryId) => {
        apiEndpoint += `&categoryIds=${categoryId}`;
      });
    }

    const response = await fetch(apiEndpoint, {
      method: "GET",
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    return response.json();
  },

  getProductDetails: async (id: string): Promise<ProductDetails> => {
    const apiEndpoint = baseApiEndpoint + `/${id}`;

    const response = await fetch(apiEndpoint, {
      method: "GET",
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    return response.json();
  },

  createProduct: async (payload: CreateProductPayload): Promise<string> => {
    const apiEndpoint = baseApiEndpoint;

    const token = authService.getToken();
    if (!token) {
      throw new Error("Not authenticated");
    }

    const response = await fetch(apiEndpoint, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    const createdProductId = await response.text();
    return createdProductId;
  },

  updateProduct: async (
    id: string,
    payload: UpdateProductPayload
  ): Promise<void> => {
    const apiEndpoint = baseApiEndpoint + `/${id}`;

    const token = authService.getToken();
    if (!token) {
      throw new Error("Not authenticated");
    }

    const response = await fetch(apiEndpoint, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }
  },

  deleteProduct: async (id: string): Promise<void> => {
    const apiEndpoint = baseApiEndpoint + `/${id}`;

    const token = authService.getToken();
    if (!token) {
      throw new Error("Not authenticated");
    }

    const response = await fetch(apiEndpoint, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }
  },
};

export default ProductService;
