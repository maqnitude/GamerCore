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
  const [selectedCategoryId, setSelectedCategoryId] = useState<string>();
  const [localProducts, setLocalProducts] = useState<Product[]>([]);

  const { paginatedList, loading, error } = useProducts(
    page,
    selectedCategoryId
  );

  const navigate = useNavigate();

  useEffect(() => {
    if (paginatedList) {
      setLocalProducts(paginatedList.items);
    }
  }, [paginatedList]);

  const handlePageChange = (page: number) => {
    setPage(page);
  };

  const handleCategoryFilterChange = (categoryId?: string) => {
    setSelectedCategoryId(categoryId);
    setPage(1);
  };

  // Update the products list and refetch if necessary
  const handleProductDeleted = (productId: string) => {
    setLocalProducts((prev) => {
      const next = prev.filter((p) => p.id !== productId);
      if (next.length === 0 && page > 1) {
        setPage(page - 1);
      }
      return next;
    });
  };

  return (
    <div className="container-fluid my-4">
      {error && (
        <ErrorAlert message={error.concat("\nFailed to fetch products.")} />
      )}

      {/* Control panel */}
      <div className="d-flex justify-content-between align-items-center">
        <ProductsFilter onCategoryFilterChange={handleCategoryFilterChange} />

        <button
          className="btn btn-primary"
          onClick={() => navigate("/products/create")}
        >
          <i className="bi bi-plus-lg me-1"></i>
          Add Product
        </button>
      </div>

      {loading && <LoadingSpinner />}
      {paginatedList && (
        <>
          <ProductsTable
            products={localProducts}
            onProductDeleted={handleProductDeleted}
          />

          <Pagination
            currentPage={paginatedList.page}
            pageSize={paginatedList.pageSize}
            totalItems={paginatedList.totalItems}
            onPageChange={handlePageChange}
          />
        </>
      )}
    </div>
  );
}

export default ProductsPage;
