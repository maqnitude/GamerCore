import { useCallback, useState } from "react";
import ProductService from "../../../services/productService";
import { CreateProductPayload } from "../../../types";

function useCreateProduct() {
  const [createdProductId, setCreatedProductId] = useState<number | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const createProduct = useCallback(async (payload: CreateProductPayload) => {
    try {
      setLoading(true);
      const id = await ProductService.createProduct(payload);
      setCreatedProductId(id);
      setError(null);

      return id;
    } catch (err) {
      console.error("Error creating product:", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  return { createProduct, createdProductId, loading, error };
}

export default useCreateProduct;