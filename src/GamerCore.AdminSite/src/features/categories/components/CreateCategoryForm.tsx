import { useForm } from "react-hook-form";
import { useToast } from "../../../contexts/ToastContext";
import useCreateCategory from "../hooks/useCreateCategory";
import { Category, CreateCategoryPayload } from "../../../types";

interface CreateCategoryFormProps {
  onClose: () => void;
  onCategoryCreated?: (createdCategory: Category) => void;
}

interface FormValues {
  name: string;
  description: string;
}

function CreateCategoryForm({
  onClose,
  onCategoryCreated,
}: CreateCategoryFormProps) {
  const { createCategory, creating, error: createError } = useCreateCategory();

  const { addToast } = useToast();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    defaultValues: {
      name: "",
      description: "",
    },
  });

  const onSubmit = async (data: FormValues) => {
    const payload: CreateCategoryPayload = {
      name: data.name,
      description: data.description,
    };

    try {
      const createdCategory = await createCategory(payload);

      addToast({
        type: "success",
        message: "Category created successfully.",
        metadata: { createdCategoryId: createdCategory.id },
        autoDismiss: true,
        dismissDelay: 5000,
      });

      // Notify categories page to update the local list
      if (onCategoryCreated) {
        onCategoryCreated(createdCategory);
      }

      onClose();
    } catch {
      addToast({
        type: "error",
        message: createError || "Failed to create category.",
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
              <h5 className="modal-title">Create New Category</h5>
              <button
                type="button"
                className="btn-close"
                aria-label="Close"
                onClick={onClose}
              ></button>
            </div>
            <div className="modal-body">
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
                disabled={isSubmitting && creating}
              >
                Cancel
              </button>
              <button
                type="submit"
                className="btn btn-primary"
                disabled={isSubmitting && creating}
              >
                {isSubmitting && creating ? "Saving..." : "Save"}
              </button>
            </div>
          </form>
        </div>
      </div>

      <div className="modal-backdrop show"></div>
    </>
  );
}

export default CreateCategoryForm;
