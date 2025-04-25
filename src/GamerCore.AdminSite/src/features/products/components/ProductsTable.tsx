import { Product } from "../../../types";

function ProductsTable({ products }: { products: Product[] }) {
  return (
    <div className="table-responsive">
      <table className="table table-striped table-hover">
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Price</th>
            <th>Rating</th>
            <th>Reviews</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {products.map((product) => (
            <tr key={product.productId}>
              <td>{product.productId}</td>
              <td>{product.name}</td>
              <td>{product.price.toFixed(2)}</td>
              <td>
                <div className="d-flex align-items-center">
                  <i className="bi bi-star-fill text-warning"></i>
                  <span className="ms-1">{product.averageRating.toFixed(2)}</span>
                </div>
              </td>
              <td>{product.reviewCount}</td>
              <td>
                <div className="btn-group btn-group-sm float-end">
                  <button className="btn btn-outline-info">
                    <i className="bi bi-eye"></i>
                  </button>
                  <button className="btn btn-outline-primary">
                    <i className="bi bi-pencil"></i>
                  </button>
                  <button className="btn btn-outline-danger">
                    <i className="bi bi-trash"></i>
                  </button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default ProductsTable;