import { useState } from "react";
import { useNavigate } from "react-router";
import ErrorAlert from "../components/ErrorAlert";
import LoadingSpinner from "../components/LoadingSpinner";
import Pagination from "../components/Pagination";
import ProductsFilter from "../features/products/components/ProductsFilter";
import ProductsTable from "../features/products/components/ProductsTable";
import useProducts from "../features/products/hooks/useProducts";

function ProductsPage() {
  const [page, setPage] = useState<number>(1);
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | undefined>();

  const { pagedResult, loading, error } = useProducts(page, selectedCategoryId);

  const navigate = useNavigate();

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

        <button
          className="btn btn-primary"
          onClick={() => navigate("/products/create")}
        >
          <i className="bi bi-plus-lg me-1"></i>
          Add Product
        </button>
      </div>

      {loading && <LoadingSpinner />}
      {error && <ErrorAlert
        message={error.concat("\nFailed to fetch products.")}
      />}

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