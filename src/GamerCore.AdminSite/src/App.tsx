import { Route, Routes } from "react-router";
import ToastContainer from "./components/ToastContainer";
import MainLayout from "./layouts/MainLayout";
import CreateProductPage from "./pages/CreateProductPage";
import DashboardPage from "./pages/DashboardPage";
import ProductsPage from "./pages/ProductsPage";
import Modal from "./components/Modal";

function App() {
  return (
    <>
      <Routes>
        <Route path="/" element={<MainLayout />}>
          <Route index element={<DashboardPage />} />
          <Route path="products" element={<ProductsPage />} />
          <Route path="products/create" element={<CreateProductPage />} />
        </Route>
      </Routes>
      <ToastContainer />
      <Modal />
    </>
  );
}

export default App;