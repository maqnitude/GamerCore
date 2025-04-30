import { useEffect, useState } from "react";
import ErrorAlert from "../components/ErrorAlert";
import LoadingSpinner from "../components/LoadingSpinner";
import CategoriesTable from "../features/categories/components/CategoriesTable";
import useCategories from "../features/categories/hooks/useCategories";
import CreateCategoryForm from "../features/categories/components/CreateCategoryForm";
import { Category } from "../types";
import UpdateCategoryForm from "../features/categories/components/UpdateCategoryForm";

function CategoriesPage() {
  const [createFormVisible, setCreateFormVisible] = useState<boolean>(false);

  const [updateFormVisible, setUpdateFormVisible] = useState<boolean>(false);
  // Need this to pass into the update form to retrieve category data
  const [updateCategoryId, setUpdateCategoryId] = useState<number | null>(null);

  const [localCategories, setLocalCategories] = useState<Category[]>([]);

  const { categories, loading, error } = useCategories();

  useEffect(() => {
    if (categories) {
      setLocalCategories(categories);
    }
  }, [categories]);

  // Called in CategoriesTable
  const handleEditButtonClick = (categoryId: number) => {
    setUpdateCategoryId(categoryId);
    setUpdateFormVisible(true);
  };

  // Update the list with the created/updated category
  // Useful for correctly displaying data like created and updated dates
  const handleCategoryCreated = (createdCategory: Category) => {
    setLocalCategories(prev => [...prev, createdCategory]);
  };

  const handleCategoryUpdated = (updatedCategory: Category) => {
    setLocalCategories(prev => prev.filter(c => c.categoryId !== updatedCategory.categoryId))
    setLocalCategories(prev => [...prev, updatedCategory]);
  }

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
      {updateFormVisible && updateCategoryId && (
        <UpdateCategoryForm
          categoryId={updateCategoryId}
          onClose={() => {
            setUpdateCategoryId(null);
            setUpdateFormVisible(false);
          }}
          onCategoryUpdated={handleCategoryUpdated}
        />
      )}

      {loading && <LoadingSpinner />}
      {localCategories && (
        <>
          <CategoriesTable
            categories={localCategories}
            onEditButtonClick={handleEditButtonClick}
            onCategoryDeleted={handleCategoryDeleted}
          />
        </>
      )}
    </div>
  );
}

export default CategoriesPage;