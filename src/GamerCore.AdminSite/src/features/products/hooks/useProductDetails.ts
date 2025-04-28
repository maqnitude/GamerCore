import { useCallback, useEffect, useState } from "react";
import { ProductDetails } from "../../../types";
import ProductService from "../../../services/productService";

function useProductDetails(productId: number) {
  const [productDetails, setProductDetails] = useState<ProductDetails>();
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const getProductDetails = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await ProductService.getProductDetails(productId);
      setProductDetails(data);
    } catch (err) {
      console.error("Error fetching product details: ", err);
      setError(err instanceof Error ? err.message : "Unknown error occured");
    } finally {
      setLoading(false);
    }
  }, [productId]);

  useEffect(() => {
    getProductDetails();
  }, [getProductDetails]);

  return { getProductDetails, productDetails, loading, error };
}

export default useProductDetails;