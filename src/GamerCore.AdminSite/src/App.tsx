import { useEffect } from "react";
import { Outlet, Route, Routes, useNavigate } from "react-router";
import Modal from "./components/Modal";
import ToastContainer from "./components/ToastContainer";
import { useAuth } from "./contexts/AuthContext";
import MainLayout from "./layouts/MainLayout";
import CategoriesPage from "./pages/CategoriesPage";
import CreateProductPage from "./pages/CreateProductPage";
import DashboardPage from "./pages/DashboardPage";
import LoginPage from "./pages/LoginPage";
import ProductDetailsPage from "./pages/ProductDetailsPage";
import ProductsPage from "./pages/ProductsPage";
import UpdateProductPage from "./pages/UpdateProductPage";
import CustomersPage from "./pages/CustomersPage";

function ProtectedRoute() {
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (!isAuthenticated) {
      navigate("/login", { replace: true });
    }
  }, [isAuthenticated, navigate]);

  return isAuthenticated ? <Outlet /> : null;
}

function RedirectRoute({ to }: { to: string }) {
  const navigate = useNavigate();

  useEffect(() => {
    navigate(to, { replace: true });
  }, [navigate, to]);

  return null;
}

function App() {
  const { isAuthenticated } = useAuth();

  return (
    <>
      <Routes>
        <Route path="/login" element={<LoginPage />} />

        {/* Protected routes using the element prop */}
        <Route element={<ProtectedRoute />}>
          <Route element={<MainLayout />}>
            <Route path="/" element={<DashboardPage />} />
            <Route path="products" element={<ProductsPage />} />
            <Route path="products/create" element={<CreateProductPage />} />
            <Route path="products/edit/:productId" element={<UpdateProductPage />} />
            <Route path="products/details/:productId" element={<ProductDetailsPage />} />
            <Route path="categories" element={<CategoriesPage />} />
            <Route path="customers" element={<CustomersPage />} />
          </Route>
        </Route>

        {/* Default route */}
        <Route path="*" element={
          isAuthenticated ? <RedirectRoute to="/" /> : <RedirectRoute to="/login" />
        } />
      </Routes>
      <ToastContainer />
      <Modal />
    </>
  );
}

export default App;