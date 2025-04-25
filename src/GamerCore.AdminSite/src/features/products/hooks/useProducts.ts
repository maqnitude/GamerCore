import { useEffect, useState } from "react";
import { PagedResult, Product } from "../../../types";
import ProductService from "../../../services/productService";

function useProducts(page: number = 1, categoryId?: number) {
  const [pagedResult, setPagedResult] = useState<PagedResult<Product>>();
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        setLoading(true);
        const data = await ProductService.getProducts(page, categoryId ? [categoryId] : undefined);
        setPagedResult(data);
        setError(null);
      } catch (err) {
        console.error("Error fetching products: ", err);
        setError(err instanceof Error ? err.message : "Unknown error occured");
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, [page, categoryId]);

  return { pagedResult, loading, error };
}

export default useProducts;