import { useFieldArray, useForm } from "react-hook-form";
import { useNavigate } from "react-router";
import ErrorAlert from "../../../components/ErrorAlert";
import LoadingSpinner from "../../../components/LoadingSpinner";
import { useToast } from "../../../contexts/ToastContext";
import { Category, CreateProductPayload } from "../../../types";
import useCategories from "../../categories/hooks/useCategories";
import useCreateProduct from "../hooks/useCreateProduct";

// API expects a list of category ids but UI can only choose 1
interface FormValues {
  name: string;
  price: number;
  descriptionHtml: string;
  warrantyHtml: string;
  categoryId: string;
  primaryImageUrl: string;
  imageUrls: { url: string }[];
}

function CreateProductForm() {
  const {
    categories,
    loading: categoriesLoading,
    error: categoriesError,
  } = useCategories();

  const { createProduct, creating, error: createError } = useCreateProduct();

  const { addToast } = useToast();

  const navigate = useNavigate();

  const { register, control, handleSubmit } = useForm<FormValues>({
    defaultValues: {
      name: "",
      price: 0,
      descriptionHtml: "",
      warrantyHtml: "",
      categoryId: "",
      primaryImageUrl: "",
      imageUrls: [{ url: "" }],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control,
    name: "imageUrls",
  });

  const onSubmit = async (data: FormValues) => {
    // Build the API payload
    const payload: CreateProductPayload = {
      name: data.name,
      price: data.price,
      descriptionHtml: data.descriptionHtml,
      warrantyHtml: data.warrantyHtml,
      categoryIds: data.categoryId ? [data.categoryId] : [],
      primaryImageUrl: data.primaryImageUrl,
      imageUrls: data.imageUrls
        .map((entry) => entry.url)
        .filter((url) => url.trim() !== ""),
    };

    try {
      const createdProductId = await createProduct(payload);

      addToast({
        type: "success",
        message: "Product created successfully.",
        metadata: { createdProductId },
        autoDismiss: true,
        dismissDelay: 5000,
      });

      navigate("/products");
    } catch {
      // Error is thrown by createError
      addToast({
        type: "error",
        message: createError || "Failed to create product.",
        autoDismiss: true,
        dismissDelay: 7500,
      });
    }
  };

  return (
    <div className="container-fluid py-3">
      <div className="card shadow-sm">
        <div className="card-header bg-light d-flex justify-content-between align-items-center py-2">
          <h5 className="mb-0">Create Product</h5>
          {/*
          <div>
            <button type="button" className="btn btn-sm btn-outline-secondary me-2" onClick={() => navigate("/products")}>
              <i className="bi bi-arrow-left me-1"></i>Back
            </button>
            <button type="button" className="btn btn-sm btn-primary" onClick={handleSubmit(onSubmit)} disabled={creating}>
              <i className="bi bi-save me-1"></i>{creating ? "Saving..." : "Save"}
            </button>
          </div>
           */}
        </div>

        {createError && (
          <div className="card-body py-2 px-3 border-bottom bg-light bg-opacity-25">
            <ErrorAlert message={createError} />
          </div>
        )}

        <div className="card-body p-3">
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="row g-3">
              {/* Left Column */}
              <div className="col-md-6">
                {/* Basic Information */}
                <div className="card">
                  <div className="card-header py-2">
                    <h6 className="mb-0">Basic Information</h6>
                  </div>
                  <div className="card-body">
                    <div className="row g-2">
                      {/* Name */}
                      <div className="col-md-8">
                        <label htmlFor="name" className="form-label small mb-1">
                          Name
                        </label>
                        <input
                          id="name"
                          className="form-control form-control-sm"
                          placeholder="Product name"
                          {...register("name", { required: true })}
                        />
                      </div>

                      {/* Price */}
                      <div className="col-md-4">
                        <label
                          htmlFor="price"
                          className="form-label small mb-1"
                        >
                          Price ($)
                        </label>
                        <input
                          id="price"
                          type="number"
                          step="0.01"
                          className="form-control form-control-sm"
                          placeholder="0.00"
                          {...register("price", {
                            required: true,
                            valueAsNumber: true,
                          })}
                        />
                      </div>

                      {/* Category Dropdown */}
                      <div className="col-12">
                        <label
                          htmlFor="category"
                          className="form-label small mb-1"
                        >
                          Category
                        </label>
                        {categoriesLoading && (
                          <div className="py-1">
                            <LoadingSpinner />
                          </div>
                        )}
                        {categoriesError && (
                          <ErrorAlert
                            message={categoriesError.concat(
                              "\nFailed to fetch categories."
                            )}
                          />
                        )}
                        {categories && (
                          <select
                            id="category"
                            className="form-select form-select-sm"
                            {...register("categoryId", { required: true })}
                          >
                            <option value="">Select a category</option>
                            {categories.map((c: Category) => (
                              <option key={c.id} value={c.id}>
                                {c.name}
                              </option>
                            ))}
                          </select>
                        )}
                      </div>
                    </div>
                  </div>
                </div>

                {/* Images Section */}
                <div className="card mt-3">
                  <div className="card-header py-2 d-flex justify-content-between align-items-center">
                    <h6 className="mb-0">Images</h6>
                  </div>
                  <div className="card-body">
                    {/* Primary Image URL */}
                    <div className="mb-2">
                      <label
                        htmlFor="primaryImageUrl"
                        className="form-label small mb-1"
                      >
                        Primary Image URL
                      </label>
                      <input
                        id="primaryImageUrl"
                        className="form-control form-control-sm"
                        placeholder="https://example.com/image.jpg"
                        {...register("primaryImageUrl", { required: true })}
                      />
                    </div>

                    {/* Additional Image URLs */}
                    <label className="form-label small mb-1">
                      Additional Images
                    </label>
                    <div
                      className="border rounded p-2 bg-light bg-opacity-50"
                      style={{ maxHeight: "200px", overflowY: "auto" }}
                    >
                      {fields.map((field, index) => (
                        <div
                          key={field.id}
                          className="input-group input-group-sm mb-1"
                        >
                          <input
                            className="form-control form-control-sm"
                            placeholder="https://example.com/image.jpg"
                            {...register(`imageUrls.${index}.url` as const)}
                          />
                          <button
                            type="button"
                            className="btn btn-outline-danger btn-sm"
                            onClick={() => remove(index)}
                          >
                            <i className="bi bi-trash"></i>
                          </button>
                        </div>
                      ))}
                    </div>
                    <button
                      type="button"
                      className="btn btn-sm btn-outline-primary mt-2"
                      onClick={() => append({ url: "" })}
                    >
                      <i className="bi bi-plus-circle me-1"></i>Add Image
                    </button>
                  </div>
                </div>
              </div>

              {/* Right Column - Content */}
              <div className="col-md-6">
                <div className="card h-100">
                  <div className="card-header py-2">
                    <h6 className="mb-0">Content</h6>
                  </div>
                  <div className="card-body">
                    {/* Description HTML */}
                    <div className="mb-3">
                      <div className="d-flex justify-content-between align-items-center mb-1">
                        <label
                          htmlFor="descriptionHtml"
                          className="form-label small mb-0"
                        >
                          Description (HTML)
                        </label>
                        <span className="badge bg-secondary">
                          HTML supported
                        </span>
                      </div>
                      <textarea
                        id="descriptionHtml"
                        rows={6}
                        className="form-control form-control-sm"
                        placeholder="<p>Enter product description here...</p>"
                        {...register("descriptionHtml")}
                      />
                    </div>

                    {/* Warranty HTML */}
                    <div className="mb-0">
                      <div className="d-flex justify-content-between align-items-center mb-1">
                        <label
                          htmlFor="warrantyHtml"
                          className="form-label small mb-0"
                        >
                          Warranty (HTML)
                        </label>
                        <span className="badge bg-secondary">
                          HTML supported
                        </span>
                      </div>
                      <textarea
                        id="warrantyHtml"
                        rows={4}
                        className="form-control form-control-sm"
                        placeholder="<p>Enter warranty information here...</p>"
                        {...register("warrantyHtml")}
                      />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </form>
        </div>

        <div className="card-footer bg-light py-2">
          <div className="d-flex justify-content-start align-items-center">
            {/*
            <small className="text-muted">All fields marked with * are required</small>
             */}
            <div>
              <button
                type="button"
                className="btn btn-sm btn-outline-secondary me-2"
                onClick={() => navigate("/products")}
              >
                Cancel
              </button>
              <button
                type="button"
                className="btn btn-sm btn-primary"
                onClick={handleSubmit(onSubmit)}
                disabled={creating}
              >
                {creating ? (
                  <>
                    <span className="spinner-border spinner-border-sm me-2"></span>
                    Saving...
                  </>
                ) : (
                  <>
                    <i className="bi bi-save me-2"></i>Save
                  </>
                )}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default CreateProductForm;
