import { useState } from "react";
import ErrorAlert from "../components/ErrorAlert";
import LoadingSpinner from "../components/LoadingSpinner";
import CategoriesTable from "../features/categories/components/CategoriesTable";
import useCategories from "../features/categories/hooks/useCategories";
import CreateCategoryForm from "../features/categories/components/CreateCategoryForm";

function CategoriesPage() {
  const [createFormVisible, setCreateFormVisible] = useState<boolean>(false);

  const { categories, loading, error } = useCategories();

  return (
    <div className="container-fluid my-4">
      {loading && <LoadingSpinner />}
      {error && <ErrorAlert message={error.concat("\nFailed to fetch products.")} />}

      <div className="d-flex justify-content-between align-items-center">
        <h2>Categories</h2>
        <button
          className="btn btn-primary"
          onClick={() => setCreateFormVisible(true)}
        >
          <i className="bi bi-plus-lg me-1"></i>
          Add Category
        </button>
      </div>

      {createFormVisible && (
        <CreateCategoryForm onClose={() => setCreateFormVisible(false)} />
      )}

      {categories && (
        <>
          <CategoriesTable categories={categories} />
        </>
      )}
    </div>
  );
}

export default CategoriesPage;