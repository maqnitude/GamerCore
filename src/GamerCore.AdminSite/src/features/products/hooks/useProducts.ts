import { useCallback, useEffect, useState } from "react";
import { PaginatedList, Product } from "../../../types";
import ProductService from "../../../services/productService";

function useProducts(page: number = 1, categoryId?: string) {
  const [paginatedList, setPaginatedList] = useState<PaginatedList<Product>>();
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const getProducts = useCallback((async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await ProductService.getProducts(page, categoryId ? [categoryId] : null);
      setPaginatedList(data);
    } catch (err) {
      console.error("Error fetching products: ", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
    } finally {
      setLoading(false);
    }
  }), [page, categoryId]);

  useEffect(() => {
    getProducts();
  }, [getProducts]);

  return { paginatedList, loading, error };
}

export default useProducts;