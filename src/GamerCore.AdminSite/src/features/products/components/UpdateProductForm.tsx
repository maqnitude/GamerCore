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

interface FormValues {
  name: string;
  price: number;
  descriptionHtml: string;
  warrantyHtml: string;
  categoryId: string;
  primaryImageUrl: string;
  imageUrls: { url: string }[];
};

function UpdateProductForm({ productId }: { productId: number }) {
  const [initialData, setInitialData] = useState<ProductDetails | null>(null);
  const [removedImageUrls, setRemovedImageUrls] = useState<string[]>([]);

  const { categories, loading: categoriesLoading, error: categoriesError } = useCategories();
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
      imageUrls: [{ url: "" }]
    }
  });
  const { fields, append, remove } = useFieldArray({
    control,
    name: "imageUrls"
  });

  // Load exiting product data
  const loadProductData = useCallback(async () => {
    try {
      const data = await ProductService.getProductDetails(productId);
      setInitialData(data);
      reset({
        name: data.name,
        price: data.price,
        descriptionHtml: data.descriptionHtml,
        warrantyHtml: data.warrantyHtml,
        categoryId: String(data.categories[0].categoryId),
        primaryImageUrl: data.images
          .find(i => i.isPrimary)?.url,
        imageUrls: data.images
          .filter(i => !i.isPrimary)
          .map(i => ({ url: i.url }))
      })
    } catch (err) {
      console.error(err);
    }
  }, [productId, reset]);

  useEffect(() => {
    loadProductData();
  }, [loadProductData]);

  const onSubmit = async (data: FormValues) => {
    const payload: UpdateProductPayload = {
      productId: productId,
      name: data.name,
      price: data.price,
      categoryIds: data.categoryId ? [Number(data.categoryId)] : [],
      descriptionHtml: data.descriptionHtml,
      warrantyHtml: data.warrantyHtml,
      primaryImageUrl: data.primaryImageUrl,
      imageUrls: data.imageUrls.map(entry => entry.url).filter(url => url.trim() !== "")
    };

    try {
      await updateProduct(productId, payload);

      addToast({
        type: "success",
        message: "Product updated successfully.",
        metadata: { productId },
        autoDismiss: true,
        dismissDelay: 5000
      });

      navigate("/products");
    } catch {
      addToast({
        type: "error",
        message: updateError || "Failed to update product.",
        autoDismiss: true,
        dismissDelay: 7500,
      })
    }
  };

  const handleRemove = (index: number) => {
    const url = fields[index].url;

    if (url && url.trim() !== "") {
      setRemovedImageUrls(prev => [...prev, url]);
    }
    remove(index);
  }

  const handleUndo = (url: string) => {
    setRemovedImageUrls(prev => prev.filter(u => u !== url));
    append({ url });
  }

  if (!initialData) {
    return <LoadingSpinner />;
  }

  return (
    <div className="container mt-4">
      <h2>Update Product (ID: {productId})</h2>
      {updateError && <ErrorAlert message={updateError} />}
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
        {categoriesError && <ErrorAlert
          message={categoriesError.concat("\nFailed to fetch categories.")}
        />}
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
          <label className="form-label">Image URLs</label>
          {fields.map((field, index) => (
            <div key={field.id} className="input-group mb-2">
              <input
                className="form-control"
                {...register(`imageUrls.${index}.url` as const)}
              />
              <button
                type="button"
                className="btn btn-outline-danger"
                onClick={() => handleRemove(index)}
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

        {/* Removed images which can be undone */}
        {removedImageUrls.length > 0 && (
          <div className="mb-3">
            <label className="form-label">Removed Image URLs</label>
            {removedImageUrls.map((url, index) => (
              <div key={index} className="d-flex align-items-center mb-1">
                <span className="me-auto">{url}</span>
                <button
                  type="button"
                  className="btn btn-sm btn-outline-secondary"
                  onClick={() => handleUndo(url)}
                >
                  Undo
                </button>
              </div>
            ))}
          </div>
        )}

        {/* Action Buttons */}
        <button type="submit" className="btn btn-primary me-2" disabled={updating}>
          {updating ? "Saving..." : "Save"}
        </button>
        <button type="button" className="btn btn-secondary" onClick={() => navigate("/products")}>
          Back
        </button>
      </form>
    </div>
  );
}

export default UpdateProductForm;