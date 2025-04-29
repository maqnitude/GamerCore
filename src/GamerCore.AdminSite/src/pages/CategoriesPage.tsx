import ErrorAlert from "../components/ErrorAlert";
import LoadingSpinner from "../components/LoadingSpinner";
import CategoriesTable from "../features/categories/components/CategoriesTable";
import useCategories from "../features/categories/hooks/useCategories";

function CategoriesPage() {
  const { categories, loading, error } = useCategories();

  return (
    <div className="container-fluid my-4">
      {loading && <LoadingSpinner />}
      {error && <ErrorAlert message={error.concat("\nFailed to fetch products.")} />}

      {categories && (
        <>
          <CategoriesTable categories={categories} />
        </>
      )}
    </div>
  );
}

export default CategoriesPage;