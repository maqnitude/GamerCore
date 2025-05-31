import { useCallback, useEffect, useState } from "react";
import { useFieldArray, useForm } from "react-hook-form";
import { useNavigate } from "react-router";
import ErrorAlert from "../../../components/ErrorAlert";
import LoadingSpinner from "../../../components/LoadingSpinner";
import { useToast } from "../../../contexts/ToastContext";
import ProductService from "../../../services/productService";
import { Category, ProductDetails, UpdateProductPayload } from "../../../types";
import useCategories from "../../categories/hooks/useCategories";
import useUpdateProduct from "../hooks/useUpdateProduct";
import { formatDate } from "../../../utils";

interface FormValues {
  name: string;
  price: number;
  descriptionHtml: string;
  warrantyHtml: string;
  categoryId: string;
  primaryImageUrl: string;
  imageUrls: { url: string }[];
}

function UpdateProductForm({ productId }: { productId: string }) {
  const [initialData, setInitialData] = useState<ProductDetails | null>(null);
  const [removedImageUrls, setRemovedImageUrls] = useState<string[]>([]);
  const [lastUpdated, setLastUpdated] = useState<string>("");

  const {
    categories,
    loading: categoriesLoading,
    error: categoriesError,
  } = useCategories();
  const { updateProduct, updating, error: updateError } = useUpdateProduct();

  const { addToast } = useToast();

  const navigate = useNavigate();

  const { register, control, handleSubmit, reset } = useForm<FormValues>({
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

  // Load exiting product data
  const loadProductData = useCallback(async () => {
    try {
      // TODO: make a custom hook for this to disable the save button on loading and error
      const data = await ProductService.getProductDetails(productId);
      setInitialData(data);
      reset({
        name: data.name,
        price: data.price,
        descriptionHtml: data.descriptionHtml,
        warrantyHtml: data.warrantyHtml,
        categoryId:
          data.categories.length != 0 ? String(data.categories[0].id) : "",
        primaryImageUrl: data.images.find((i) => i.isPrimary)?.url,
        imageUrls: data.images
          .filter((i) => !i.isPrimary)
          .map((i) => ({ url: i.url })),
      });
      setLastUpdated(formatDate(data.updatedAt));
    } catch (err) {
      console.error(err);
      // TODO: add error toast
    }
  }, [productId, reset]);

  useEffect(() => {
    loadProductData();
  }, [loadProductData]);

  const onSubmit = async (data: FormValues) => {
    const payload: UpdateProductPayload = {
      id: productId,
      name: data.name,
      price: data.price,
      categoryIds: data.categoryId ? [data.categoryId] : [],
      descriptionHtml: data.descriptionHtml,
      warrantyHtml: data.warrantyHtml,
      primaryImageUrl: data.primaryImageUrl,
      imageUrls: data.imageUrls
        .map((entry) => entry.url)
        .filter((url) => url.trim() !== ""),
    };

    try {
      await updateProduct(productId, payload);

      addToast({
        type: "success",
        message: "Product updated successfully.",
        metadata: { updatedProductId: productId },
        autoDismiss: true,
        dismissDelay: 5000,
      });

      navigate("/products");
    } catch {
      addToast({
        type: "error",
        message: updateError || "Failed to update product.",
        autoDismiss: true,
        dismissDelay: 7500,
      });
    }
  };

  const handleRemove = (index: number) => {
    const url = fields[index].url;

    if (url && url.trim() !== "") {
      setRemovedImageUrls((prev) => [...prev, url]);
    }
    remove(index);
  };

  const handleUndo = (url: string) => {
    setRemovedImageUrls((prev) => prev.filter((u) => u !== url));
    append({ url });
  };

  if (!initialData) {
    return <LoadingSpinner />;
  }

  return (
    <div className="container-fluid py-3">
      <div className="card shadow-sm">
        <div className="card-header bg-light d-flex justify-content-between align-items-center py-2">
          <div>
            <h5 className="mb-0">Update Product</h5>
            <small className="text-muted">
              ID: {productId} - {initialData.name}
            </small>
          </div>
          {/*
          <div>
            <button type="button" className="btn btn-sm btn-outline-secondary me-2" onClick={() => navigate("/products")}>
              <i className="bi bi-arrow-left me-1"></i>Back
            </button>
            <button type="button" className="btn btn-sm btn-primary" onClick={handleSubmit(onSubmit)} disabled={updating}>
              <i className="bi bi-save me-1"></i>{updating ? "Saving..." : "Save"}
            </button>
          </div>
           */}
        </div>

        {updateError && (
          <div className="card-body py-2 px-3 border-bottom bg-light bg-opacity-25">
            <ErrorAlert message={updateError} />
          </div>
        )}

        <div className="card-body p-3">
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="row g-3">
              {/* Left Column */}
              <div className="col-md-6">
                {/* Basic Information */}
                <div className="card mb-3">
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
                <div className="card">
                  <div className="card-header py-2 d-flex justify-content-between align-items-center">
                    <h6 className="mb-0">Images</h6>
                    <span className="badge bg-info">
                      {fields.length + 1} total
                    </span>
                  </div>
                  <div className="card-body">
                    {/* Primary Image URL with preview */}
                    <div className="mb-3">
                      <div className="d-flex justify-content-between align-items-center mb-1">
                        <label
                          htmlFor="primaryImageUrl"
                          className="form-label small mb-0"
                        >
                          Primary Image URL
                        </label>
                        <a
                          href="#"
                          className="btn btn-sm btn-link py-0"
                          onClick={(e) => {
                            e.preventDefault();
                            window.open(
                              initialData.images.find((i) => i.isPrimary)?.url,
                              "_blank"
                            );
                          }}
                        >
                          Preview
                        </a>
                      </div>
                      <input
                        id="primaryImageUrl"
                        className="form-control form-control-sm"
                        {...register("primaryImageUrl", { required: true })}
                      />
                    </div>

                    {/* Additional Image URLs */}
                    <label className="form-label small mb-1">
                      Additional Images
                    </label>
                    <div className="border rounded p-2 bg-light bg-opacity-50">
                      {fields.map((field, index) => (
                        <div
                          key={field.id}
                          className="input-group input-group-sm mb-1"
                        >
                          <input
                            className="form-control form-control-sm"
                            {...register(`imageUrls.${index}.url` as const)}
                          />
                          <button
                            type="button"
                            className="btn btn-outline-secondary btn-sm"
                            onClick={(e) => {
                              e.preventDefault();
                              window.open(field.url, "_blank");
                            }}
                            title="Preview image"
                          >
                            <i className="bi bi-eye"></i>
                          </button>
                          <button
                            type="button"
                            className="btn btn-outline-danger btn-sm"
                            onClick={() => handleRemove(index)}
                            title="Remove image"
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

                    {/* Removed images section */}
                    {removedImageUrls.length > 0 && (
                      <div className="mt-3">
                        <div className="d-flex justify-content-between align-items-center mb-1">
                          <label className="form-label small mb-0">
                            Removed Images
                          </label>
                          <span className="badge bg-warning text-dark">
                            {removedImageUrls.length}
                          </span>
                        </div>
                        <div
                          className="border border-warning rounded p-2 bg-warning bg-opacity-10"
                          style={{ maxHeight: "100px", overflowY: "auto" }}
                        >
                          {removedImageUrls.map((url, index) => (
                            <div
                              key={index}
                              className="d-flex align-items-center mb-1 small"
                            >
                              <div className="text-truncate me-2" title={url}>
                                {url}
                              </div>
                              <button
                                type="button"
                                className="btn btn-sm btn-outline-success py-0 px-1 ms-auto"
                                onClick={() => handleUndo(url)}
                              >
                                <i className="bi bi-arrow-counterclockwise"></i>{" "}
                                Restore
                              </button>
                            </div>
                          ))}
                        </div>
                      </div>
                    )}
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
                        {...register("descriptionHtml")}
                      />
                      <div
                        className="mt-2 border rounded p-2 bg-light"
                        style={{ maxHeight: "256px", overflowY: "auto" }}
                      >
                        <small className="text-muted">Preview:</small>
                        <div
                          dangerouslySetInnerHTML={{
                            __html: initialData.descriptionHtml,
                          }}
                        />
                      </div>
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
                        {...register("warrantyHtml")}
                      />
                      <div
                        className="mt-2 border rounded p-2 bg-light"
                        style={{ maxHeight: "256px", overflowY: "auto" }}
                      >
                        <small className="text-muted">Preview:</small>
                        <div
                          dangerouslySetInnerHTML={{
                            __html: initialData.warrantyHtml,
                          }}
                        />
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </form>
        </div>

        <div className="card-footer bg-light py-2">
          <div className="d-flex justify-content-between align-items-center">
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
                disabled={updating}
              >
                {updating ? (
                  <>
                    <span className="spinner-border spinner-border-sm me-2"></span>
                    Saving...
                  </>
                ) : (
                  <>
                    <i className="bi bi-save me-2"></i>Update
                  </>
                )}
              </button>
            </div>
            <div>
              <small className="text-muted">Last updated: {lastUpdated}</small>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default UpdateProductForm;
