import { useCallback, useEffect, useState } from "react";
import { PagedResult, Product } from "../../../types";
import ProductService from "../../../services/productService";

function useProducts(page: number = 1, categoryId?: number) {
  const [pagedResult, setPagedResult] = useState<PagedResult<Product>>();
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetchProducts = useCallback((async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await ProductService.getProducts(page, categoryId ? [categoryId] : undefined);
      setPagedResult(data);
    } catch (err) {
      console.error("Error fetching products: ", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
    } finally {
      setLoading(false);
    }
  }), [page, categoryId]);

  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]);

  return { pagedResult, loading, error };
}

export default useProducts;