import { useParams } from "react-router";
import UpdateProductForm from "../features/products/components/UpdateProductForm";

function UpdateProductPage() {
  const { productId } = useParams<{ productId: string }>();

  return (
    <div>
      <UpdateProductForm productId={productId!} />
    </div>
  );
}

export default UpdateProductPage;
