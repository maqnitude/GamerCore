import { Route, Routes } from "react-router";
import MainLayout from "./layouts/MainLayout";
import CreateProductPage from "./pages/CreateProductPage";
import DashboardPage from "./pages/DashboardPage";
import ProductsPage from "./pages/ProductsPage";
import Modal from "./components/Modal";
import ToastContainer from "./components/ToastContainer";
import UpdateProductPage from "./pages/UpdateProductPage";
import ProductDetailsPage from "./pages/ProductDetailsPage";
import CategoriesPage from "./pages/CategoriesPage";

function App() {
  return (
    <>
      <Routes>
        <Route path="/" element={<MainLayout />}>
          <Route index element={<DashboardPage />} />

          <Route path="products" element={<ProductsPage />} />
          <Route path="products/create" element={<CreateProductPage />} />
          <Route path="products/edit/:productId" element={<UpdateProductPage />} />
          <Route path="products/details/:productId" element={<ProductDetailsPage />} />

          <Route path="categories" element={<CategoriesPage />} />
        </Route>
      </Routes>
      <ToastContainer />
      <Modal />
    </>
  );
}

export default App;