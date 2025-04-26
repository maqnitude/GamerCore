import { CreateProductPayload, PagedResult, Product } from "../types";

const baseApiEndpoint = "/api/products";

const ProductService = {
  getProducts: async (page: number = 1, categoryIds?: number[]): Promise<PagedResult<Product>> => {
    let apiEndpoint = baseApiEndpoint;

    apiEndpoint += `?page=${page}`

    if (categoryIds) {
      categoryIds.forEach((categoryId) => {
        apiEndpoint += `&categoryIds=${categoryId}`;
      });
    }

    const response = await fetch(apiEndpoint);

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    return response.json();
  },

  createProduct: async (payload: CreateProductPayload): Promise<number> => {
    const apiEndpoint = baseApiEndpoint;

    const response = await fetch(apiEndpoint, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    if (!response.ok) {
      throw new Error(`${response.status} - ${response.statusText}`);
    }

    return response.json();
  }
};

export default ProductService;