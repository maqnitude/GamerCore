import { useFieldArray, useForm } from "react-hook-form";
import useCategories from "../../categories/hooks/useCategories";
import { Category, CreateProductPayload } from "../../../types";
import useCreateProduct from "../hooks/useCreateProduct";
import { useNavigate } from "react-router";
import LoadingSpinner from "../../../components/LoadingSpinner";

// API expects a list of category ids but UI can only choose 1 (bad but will be fixed in the future)
interface FormValues {
  name: string;
  price: number;
  descriptionHtml: string;
  warrantyHtml: string;
  categoryId: string;
  primaryImageUrl: string;
  imageUrls: { url: string }[];
};

function CreateProductForm() {
  const { categories, loading: categoriesLoading, error: categoriesError } = useCategories();
  const { register, control, handleSubmit } = useForm<FormValues>({
    defaultValues: {
      name: "",
      price: 0,
      descriptionHtml: "",
      warrantyHtml: "",
      categoryId: "",
      primaryImageUrl: "",
      imageUrls: [{ url: "" }]
    }
  });
  const { fields, append, remove } = useFieldArray({
    control,
    name: "imageUrls"
  });

  const { createProduct, loading: creating, error: createError } = useCreateProduct();
  const navigate = useNavigate();

  const onSubmit = async (data: FormValues) => {
    // Build the API payload
    const payload: CreateProductPayload = {
      name: data.name,
      price: data.price,
      descriptionHtml: data.descriptionHtml,
      warrantyHtml: data.warrantyHtml,
      categoryIds: data.categoryId ? [Number(data.categoryId)] : [],
      primaryImageUrl: data.primaryImageUrl,
      imageUrls: data.imageUrls.map(entry => entry.url).filter(url => url.trim() !== "")
    };

    try {
      const id = await createProduct(payload);
      navigate("/products", {
        state: {
          toast: {
            message: "Product created successfully.",
            id
          }
        }
      });
    } catch {
      // error is handled by createError
    }
  };

  return (
    <div className="container mt-4">
      <h2>Create Product</h2>
      {(createError || categoriesError) && <div className="alert alert-danger">{createError || categoriesError}</div>}
      <form onSubmit={handleSubmit(onSubmit)}>
        {/* Name */}
        <div className="mb-3">
          <label htmlFor="name" className="form-label">Name</label>
          <input
            id="name"
            className="form-control"
            {...register('name', { required: true })}
          />
        </div>

        {/* Price */}
        <div className="mb-3">
          <label htmlFor="price" className="form-label">Price</label>
          <input
            id="price"
            type="number"
            step="0.01"
            className="form-control"
            {...register('price', { required: true, valueAsNumber: true })}
          />
        </div>

        {/* Category Dropdown */}
        {categoriesLoading && <LoadingSpinner />}
        {categories && (
          <>
            <div className="mb-3">
              <label htmlFor="category" className="form-label">Category</label>
              <select
                id="category"
                className="form-select"
                {...register('categoryId', { required: true })}
              >
                <option value="">Select a category</option>
                {categories.map((c: Category) => (
                  <option key={c.categoryId} value={c.categoryId}>
                    {c.name}
                  </option>
                ))}
              </select>
            </div>
          </>
        )}

        {/* Description HTML */}
        <div className="mb-3">
          <label htmlFor="descriptionHtml" className="form-label">Description (HTML)</label>
          <textarea
            id="descriptionHtml"
            rows={3}
            className="form-control"
            {...register('descriptionHtml')}
          />
        </div>

        {/* Warranty HTML */}
        <div className="mb-3">
          <label htmlFor="warrantyHtml" className="form-label">Warranty (HTML)</label>
          <textarea
            id="warrantyHtml"
            rows={2}
            className="form-control"
            {...register('warrantyHtml')}
          />
        </div>

        {/* Primary Image URL */}
        <div className="mb-3">
          <label htmlFor="primaryImageUrl" className="form-label">Primary Image URL</label>
          <input
            id="primaryImageUrl"
            className="form-control"
            {...register('primaryImageUrl', { required: true })}
          />
        </div>

        {/* Optional Additional Images */}
        <div className="mb-3">
          <label className="form-label">Other Image URLs</label>
          {fields.map((field, index) => (
            <div key={field.id} className="input-group mb-2">
              <input
                className="form-control"
                {...register(`imageUrls.${index}.url` as const)}
              />
              <button
                type="button"
                className="btn btn-outline-danger"
                onClick={() => remove(index)}
              >
                Remove
              </button>
            </div>
          ))}
          <button
            type="button"
            className="btn btn-outline-primary"
            onClick={() => append({ url: "" })}
          >
            Add Image
          </button>
        </div>

        {/* Action Buttons */}
        <button type="submit" className="btn btn-primary me-2" disabled={creating}>
          {creating ? "Saving..." : "Save"}
        </button>
        <button type="button" className="btn btn-secondary" onClick={() => navigate("/products")}>
          Back
        </button>
      </form>
    </div>
  );
}

export default CreateProductForm;