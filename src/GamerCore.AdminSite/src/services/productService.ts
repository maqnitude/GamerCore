import { PagedResult, Product } from "../types";

const ProductService = {
  getProducts: async (page: number = 1, categoryIds?: number[]): Promise<PagedResult<Product>> => {
    let apiEndpoint = "/api/products";

    apiEndpoint += `?page=${page}`

    if (categoryIds) {
      categoryIds.forEach((categoryId) => {
        apiEndpoint += `&categoryIds=${categoryId}`;
      });
    }

    const response = await fetch(apiEndpoint);

    if (!response.ok) {
      throw new Error(`Network error - Status: ${response.status}`);
    }

    return response.json();
  }
};

export default ProductService;