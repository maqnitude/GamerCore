import { useCallback, useState } from "react";
import { UpdateProductPayload } from "../../../types";
import ProductService from "../../../services/productService";

function useUpdateProduct() {
  const [updating, setUpdating] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const updateProduct = useCallback(async (id: string, payload: UpdateProductPayload) => {
    try {
      setUpdating(true);
      await ProductService.updateProduct(id, payload);
      setError(null);
    } catch (err) {
      console.error("Error creating product:", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
      throw err;
    } finally {
      setUpdating(false);
    }
  }, []);

  return { updateProduct, updating, error };
}

export default useUpdateProduct;