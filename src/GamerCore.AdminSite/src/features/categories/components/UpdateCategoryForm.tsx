import { useForm } from "react-hook-form";
import { useToast } from "../../../contexts/ToastContext";
import useUpdateCategory from "../hooks/useUpdateCategory";
import { useCallback, useEffect, useState } from "react";
import CategoryService from "../../../services/categoryService";
import { Category, UpdateCategoryPayload } from "../../../types";
import LoadingSpinner from "../../../components/LoadingSpinner";

interface UpdateCategoryFormProps {
  categoryId: string;
  onClose: () => void;
  onCategoryUpdated?: (updatedCategory: Category) => void;
}

interface FormValues {
  name: string;
  description: string;
}

function UpdateCategoryForm({
  categoryId,
  onClose,
  onCategoryUpdated,
}: UpdateCategoryFormProps) {
  const [initialData, setInitialData] = useState<Category | null>(null);

  const { updateCategory, updating, error: updateError } = useUpdateCategory();

  const { addToast } = useToast();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    defaultValues: {
      name: "",
      description: "",
    },
  });

  const loadCategoryData = useCallback(async () => {
    try {
      const data = await CategoryService.getCategory(categoryId);
      setInitialData(data);
      reset({
        name: data.name,
        description: data.description,
      });
    } catch (err) {
      console.error(err);

      // Create error toast then close
      addToast({
        type: "error",
        message: "Failed to fetch category data.",
        autoDismiss: true,
        dismissDelay: 7500,
      });

      onClose();
    }
  }, [addToast, categoryId, onClose, reset]);

  useEffect(() => {
    loadCategoryData();
  }, [loadCategoryData]);

  const onSubmit = async (data: FormValues) => {
    const payload: UpdateCategoryPayload = {
      id: categoryId,
      name: data.name,
      description: data.description,
    };

    try {
      const updatedCategory = await updateCategory(categoryId, payload);

      addToast({
        type: "success",
        message: "Category created successfully.",
        metadata: { updatedCategory },
        autoDismiss: true,
        dismissDelay: 5000,
      });

      // Notify categories page to update the local list
      if (onCategoryUpdated) {
        onCategoryUpdated(updatedCategory);
      }

      onClose();
    } catch {
      addToast({
        type: "error",
        message: updateError || "Failed to update category.",
        autoDismiss: true,
        dismissDelay: 7500,
      });
    }
  };

  return (
    <>
      <div className="modal fade d-block show" tabIndex={-1}>
        <div className="modal-dialog modal-dialog-centered">
          <form className="modal-content" onSubmit={handleSubmit(onSubmit)}>
            <div className="modal-header">
              <h5 className="modal-title">Update Category</h5>
              <button
                type="button"
                className="btn-close"
                aria-label="Close"
                onClick={onClose}
              ></button>
            </div>
            <div className="modal-body">
              {!initialData && <LoadingSpinner />}

              {/* Name field */}
              <div className="mb-3">
                <label htmlFor="category-name" className="form-label">
                  Name
                </label>
                <input
                  id="category-name"
                  type="text"
                  className={`form-control ${errors.name ? "is-invalid" : ""}`}
                  {...register("name", { required: "Name is required" })}
                />
                {errors.name && (
                  <div className="invalid-feedback">{errors.name.message}</div>
                )}
              </div>

              {/* Description field */}
              <div className="mb-3">
                <label htmlFor="category-description" className="form-label">
                  Description
                </label>
                <textarea
                  id="category-description"
                  rows={3}
                  className={`form-control ${
                    errors.description ? "is-invalid" : ""
                  }`}
                  {...register("description", {
                    required: false,
                  })}
                />
                {errors.description && (
                  <div className="invalid-feedback">
                    {errors.description.message}
                  </div>
                )}
              </div>
            </div>
            <div className="modal-footer">
              <button
                type="button"
                className="btn btn-secondary"
                onClick={onClose}
                disabled={isSubmitting && updating}
              >
                Cancel
              </button>
              <button
                type="submit"
                className="btn btn-primary"
                disabled={isSubmitting && updating}
              >
                {isSubmitting && updating ? "Saving..." : "Save"}
              </button>
            </div>
          </form>
        </div>
      </div>

      <div className="modal-backdrop show"></div>
    </>
  );
}

export default UpdateCategoryForm;
