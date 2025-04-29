import { useEffect, useState } from "react";
import ErrorAlert from "../components/ErrorAlert";
import LoadingSpinner from "../components/LoadingSpinner";
import CategoriesTable from "../features/categories/components/CategoriesTable";
import useCategories from "../features/categories/hooks/useCategories";
import CreateCategoryForm from "../features/categories/components/CreateCategoryForm";
import { Category } from "../types";

function CategoriesPage() {
  const [createFormVisible, setCreateFormVisible] = useState<boolean>(false);
  const [localCategories, setLocalCategories] = useState<Category[]>([]);

  const { categories, loading, error } = useCategories();

  useEffect(() => {
    if (categories) {
      setLocalCategories(categories);
    }
  }, [categories]);

  // Update the list with the new category (useful for correctly displaying data like created and updated dates)
  const handleCategoryCreated = (newCategory: Category) => {
    setLocalCategories(prev => [...prev, newCategory])
  };

  const handleCategoryDeleted = (categoryId: number) => {
    setLocalCategories(prev => prev.filter(c => c.categoryId !== categoryId));
  };

  return (
    <div className="container-fluid my-4">
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
        <CreateCategoryForm
          onClose={() => setCreateFormVisible(false)}
          onCategoryCreated={handleCategoryCreated}
        />
      )}

      {loading && <LoadingSpinner />}
      {localCategories && (
        <>
          <CategoriesTable
            categories={localCategories}
            onCategoryDeleted={handleCategoryDeleted}
          />
        </>
      )}
    </div>
  );
}

export default CategoriesPage;