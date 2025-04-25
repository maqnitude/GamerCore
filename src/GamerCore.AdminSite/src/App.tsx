import { Route, Routes } from "react-router";
import ProductsPage from "./pages/ProductsPage";
import MainLayout from "./layouts/MainLayout";
import DashboardPage from "./pages/DashboardPage";
import CreateProductPage from "./pages/CreateProductPage";

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
    </>
  );
}

export default App;