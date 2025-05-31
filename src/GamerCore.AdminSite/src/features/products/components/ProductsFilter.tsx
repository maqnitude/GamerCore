import React, { useState } from "react";
import ErrorAlert from "../../../components/ErrorAlert";
import LoadingSpinner from "../../../components/LoadingSpinner";
import useCategories from "../../categories/hooks/useCategories";

interface ProductsFilterProps {
  onCategoryFilterChange: (categoryId?: string) => void;
}

function ProductsFilter({ onCategoryFilterChange }: ProductsFilterProps) {
  const [selectedCategoryId, setSelectedCategoryId] = useState<string>();
  const {
    categories,
    loading: categoriesLoading,
    error: categoriesError,
  } = useCategories();

  const handleCategoryChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const categoryId = e.target.value === "" ? undefined : e.target.value;
    setSelectedCategoryId(categoryId);
    onCategoryFilterChange(categoryId);
  };

  return (
    <div className="card">
      <div className="card-body py-2">
        <div className="row align-items-center">
          <div className="col-2">
            <div className="d-flex flex-row align-items-center">
              <i className="bi bi-funnel-fill me-2 text-primary fs-5"></i>
            </div>
          </div>
          <div className="col">
            <div className="form-group d-flex flex-row align-items-center">
              <label
                htmlFor="categoryFilter"
                className="form-label fw-bold me-2"
              >
                Category:
              </label>
              <select
                id="categoryFilter"
                className="form-select"
                value={selectedCategoryId?.toString() || ""}
                onChange={handleCategoryChange}
                disabled={categoriesLoading}
              >
                <option value="">All Categories</option>
                {categories.map((category) => (
                  <option key={category.id} value={category.id.toString()}>
                    {category.name}
                  </option>
                ))}
              </select>

              {categoriesLoading && <LoadingSpinner />}
              {categoriesError && (
                <ErrorAlert
                  message={categoriesError.concat(
                    "\nFailed to fetch categories."
                  )}
                />
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ProductsFilter;
