import { useCallback, useState } from "react";
import ProductService from "../../../services/productService";
import { CreateProductPayload } from "../../../types";

function useCreateProduct() {
  const [createdProductId, setCreatedProductId] = useState<string | null>(null);
  const [creating, setCreating] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const createProduct = useCallback(async (payload: CreateProductPayload) => {
    try {
      setCreating(true);
      const id = await ProductService.createProduct(payload);
      setCreatedProductId(id);
      setError(null);

      return id;
    } catch (err) {
      console.error("Error creating product:", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
      throw err;
    } finally {
      setCreating(false);
    }
  }, []);

  return { createProduct, createdProductId, creating, error };
}

export default useCreateProduct;