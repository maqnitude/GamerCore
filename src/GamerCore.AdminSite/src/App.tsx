import { Route, Routes } from "react-router";
import ProductsPage from "./pages/ProductsPage";
import MainLayout from "./layouts/MainLayout";
import DashboardPage from "./pages/DashboardPage";

function App() {
  return (
    <>
      <Routes>
        <Route path="/" element={<MainLayout />}>
          <Route index element={<DashboardPage />} />
          <Route path="products" element={<ProductsPage />} />
        </Route>
      </Routes>
    </>
  );
}

export default App;