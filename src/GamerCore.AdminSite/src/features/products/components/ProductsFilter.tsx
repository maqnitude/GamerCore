import React, { useState } from "react";
import ErrorAlert from "../../../components/ErrorAlert";
import LoadingSpinner from "../../../components/LoadingSpinner";
import useCategories from "../../categories/hooks/useCategories";

interface ProductsFilterProps {
  onFilterChange: (categoryId: number | undefined) => void;
}

function ProductsFilter({ onFilterChange }: ProductsFilterProps) {
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | undefined>();
  const { categories, loading: categoriesLoading, error: categoriesError } = useCategories();

  const handleCategoryChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const categoryId = e.target.value === "" ? undefined : Number(e.target.value);
    setSelectedCategoryId(categoryId);
    onFilterChange(categoryId);
  }

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
              <label htmlFor="categoryFilter" className="form-label fw-bold me-2">Category:</label>
              <select
                id="categoryFilter"
                className="form-select"
                value={selectedCategoryId?.toString() || ""}
                onChange={handleCategoryChange}
                disabled={categoriesLoading}
              >
                <option value="">All Categories</option>
                {categories.map(category => (
                  <option key={category.categoryId} value={category.categoryId.toString()}>
                    {category.name}
                  </option>
                ))}
              </select>

              {categoriesLoading && <LoadingSpinner />}
              {categoriesError && <ErrorAlert
                message={categoriesError.concat("\nFailed to fetch categories.")}
              />}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ProductsFilter;