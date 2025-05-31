import { useCallback, useEffect, useState } from "react";
import { ProductDetails } from "../../../types";
import ProductService from "../../../services/productService";

function useProductDetails(id: string) {
  const [productDetails, setProductDetails] = useState<ProductDetails>();
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const getProductDetails = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await ProductService.getProductDetails(id);
      setProductDetails(data);
    } catch (err) {
      console.error("Error fetching product details: ", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    getProductDetails();
  }, [getProductDetails]);

  return { getProductDetails, productDetails, loading, error };
}

export default useProductDetails;