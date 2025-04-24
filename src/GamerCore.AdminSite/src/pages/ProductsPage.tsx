import { useState } from "react";
import useProducts from "../features/products/hooks/useProducts";
import LoadingSpinner from "../components/LoadingSpinner";
import ErrorMessage from "../components/ErrorMessage";
import ProductsTable from "../features/products/components/ProductsTable";
import Pagination from "../components/Pagination";
import ProductsFilter from "../features/products/components/ProductsFilter";

function ProductsPage() {
  const [page, setPage] = useState<number>(1);
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | undefined>();
  const { pagedResult, loading, error } = useProducts(page, selectedCategoryId);

  const handlePageChange = (page: number) => {
    setPage(page);
  }

  const handleFilterChange = (categoryId: number | undefined) => {
    setSelectedCategoryId(categoryId);
    setPage(1);
  }

  return (
    <div className="container-fluid">
      {/* Control panel */}
      <div className="d-flex justify-content-between align-items-center">
        <ProductsFilter onFilterChange={handleFilterChange} />

        <button className="btn btn-primary">
          <i className="bi bi-plus-lg me-1"></i>
          Add Product
        </button>
      </div>

      {loading && <LoadingSpinner />}
      {error && <ErrorMessage message={error} />}

      {pagedResult && (
        <>
          <ProductsTable products={pagedResult.items} />

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