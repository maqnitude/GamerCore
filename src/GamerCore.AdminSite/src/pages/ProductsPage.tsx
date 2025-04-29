import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import ErrorAlert from "../components/ErrorAlert";
import LoadingSpinner from "../components/LoadingSpinner";
import Pagination from "../components/Pagination";
import ProductsFilter from "../features/products/components/ProductsFilter";
import ProductsTable from "../features/products/components/ProductsTable";
import useProducts from "../features/products/hooks/useProducts";
import { Product } from "../types";

function ProductsPage() {
  const [page, setPage] = useState<number>(1);
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | undefined>();
  const [localProducts, setLocalProducts] = useState<Product[]>([]);

  const { pagedResult, loading, error } = useProducts(page, selectedCategoryId);

  const navigate = useNavigate();

  useEffect(() => {
    if (pagedResult) {
      setLocalProducts(pagedResult.items);
    }
  }, [pagedResult]);

  const handlePageChange = (page: number) => {
    setPage(page);
  }

  const handleFilterChange = (categoryId: number | undefined) => {
    setSelectedCategoryId(categoryId);
    setPage(1);
  }

  // Update the products list and refetch if necessary
  const handleProductDeleted = (productId: number) => {
    setLocalProducts(prev => {
      const next = prev.filter(p => p.productId !== productId);
      if (next.length === 0 && page > 1) {
        setPage(page - 1);
      }
      return next;
    });
  }

  return (
    <div className="container-fluid my-4">
      {loading && <LoadingSpinner />}
      {error && <ErrorAlert message={error.concat("\nFailed to fetch products.")} />}

      {/* Control panel */}
      <div className="d-flex justify-content-between align-items-center">
        <ProductsFilter onFilterChange={handleFilterChange} />

        <button
          className="btn btn-primary"
          onClick={() => navigate("/products/create")}
        >
          <i className="bi bi-plus-lg me-1"></i>
          Add Product
        </button>
      </div>

      {pagedResult && (
        <>
          <ProductsTable
            products={localProducts}
            onProductDeleted={handleProductDeleted}
          />

          <Pagination
            currentPage={pagedResult.page}
            pageSize={pagedResult.pageSize}
            totalItems={pagedResult.totalItems}
            onPageChange={handlePageChange}
          />
        </>
      )}
    </div>
  );
}

export default ProductsPage;