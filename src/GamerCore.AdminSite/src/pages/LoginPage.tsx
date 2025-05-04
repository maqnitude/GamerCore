import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router";
import { LoginPayload } from "../types";
import ErrorAlert from "../components/ErrorAlert";
import { useAuth } from "../contexts/AuthContext";
import { useToast } from "../contexts/ToastContext";

interface FormValues {
  email: string;
  password: string;
  rememberMe: boolean;
}

function LoginPage() {
  const [error, setError] = useState<string>("");

  const { isAuthenticated, login } = useAuth();
  const { addToast } = useToast();

  const navigate = useNavigate();

  useEffect(() => {
    if (isAuthenticated) {
      navigate("/", { replace: true });
    }
  }, [isAuthenticated, navigate]);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }
  } = useForm<FormValues>({
    defaultValues: {
      email: "",
      password: "",
      rememberMe: false
    }
  });

  const onSubmit = async (data: FormValues) => {
    const payload: LoginPayload = {
      email: data.email,
      password: data.password,
      rememberMe: data.rememberMe
    };

    setError("");
    try {
      await login(payload);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : "Login failed";
      setError(errorMessage);

      addToast({
        type: "error",
        message: errorMessage,
        autoDismiss: true,
        dismissDelay: 7500
      });
    }
  };

  return (
    <div className="container vh-100 d-flex align-items-center justify-content-center">
      <div className="card shadow-sm w-100" style={{ maxWidth: '400px' }}>
        <div className="card-body">
          <h3 className="card-title text-center mb-4">Admin Login</h3>

          {error && <ErrorAlert message={error} />}

          <form onSubmit={handleSubmit(onSubmit)} noValidate>
            <div className="mb-3">
              <label htmlFor="email" className="form-label">
                Email address
              </label>
              <input
                id="email"
                type="email"
                className={`form-control ${errors.email ? "is-invalid" : ""}`}
                {...register("email", {
                  required: "Email is required",
                  pattern: {
                    value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                    message: "Invalid email address"
                  }
                })}
              />
              {errors.email && (
                <div className="invalid-feedback">{errors.email.message}</div>
              )}
            </div>

            <div className="mb-3">
              <label htmlFor="password" className="form-label">
                Password
              </label>
              <input
                id="password"
                type="password"
                className={`form-control ${errors.password ? "is-invalid" : ""}`}
                {...register("password", {
                  required: "Password is required",
                  minLength: {
                    value: 6,
                    message: "Password must be at least 6 characters"
                  }
                })}
              />
              {errors.password && (
                <div className="invalid-feedback">{errors.password.message}</div>
              )}
            </div>

            <div className="form-check mb-4">
              <input
                id="rememberMe"
                type="checkbox"
                className="form-check-input"
                {...register("rememberMe")}
              />
              <label htmlFor="rememberMe" className="form-check-label">
                Remember me
              </label>
            </div>

            <button
              type="submit"
              className="btn btn-primary w-100"
              disabled={isSubmitting}
            >
              {isSubmitting && (
                <span
                  className="spinner-border spinner-border-sm me-2"
                  role="status"
                  aria-hidden="true"
                ></span>
              )}
              {isSubmitting ? "Logging in..." : "Log In"}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}

export default LoginPage;