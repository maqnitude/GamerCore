import { useNavigate, useParams } from "react-router";
import useProductDetails from "../features/products/hooks/useProductDetails";
import LoadingSpinner from "../components/LoadingSpinner";
import ErrorAlert from "../components/ErrorAlert";
import { useState } from "react";

function ProductDetailsPage() {
  const { productId } = useParams<{ productId: string }>();
  const { productDetails, loading, error } = useProductDetails(productId!);

  const [currentIndex, setCurrentIndex] = useState<number>(0);

  const navigate = useNavigate();

  const prevSlide = () => {
    if (productDetails) {
      const length = productDetails.images.length;
      setCurrentIndex((i) => (i - 1 + length) % length);
    }
  };
  const nextSlide = () => {
    if (productDetails) {
      const length = productDetails.images.length;
      setCurrentIndex((i) => (i + 1) % length);
    }
  };

  if (!productDetails || loading) return <LoadingSpinner />;
  if (error)
    return (
      <ErrorAlert message={`${error}\nFailed to fetch product details.`} />
    );

  return (
    <div className="container-fluid py-3">
      <div className="card">
        <div className="card-header bg-light d-flex justify-content-between align-items-center">
          <h5 className="mb-0">{productDetails.name}</h5>
          <div>
            <span className="badge bg-primary me-2">
              ${productDetails.price.toFixed(2)}
            </span>
            <span className="badge bg-secondary">
              <i className="bi bi-star-fill"></i>{" "}
              {productDetails.averageRating.toFixed(1)} (
              {productDetails.reviewCount})
            </span>
          </div>
        </div>

        <div className="card-body p-3">
          <div className="row g-3">
            {/* Left column - Images */}
            <div className="col-md-4">
              <div className="card h-100">
                <div className="card-header py-2">
                  <h6 className="mb-0">Product Images</h6>
                </div>
                <div className="card-body p-0 position-relative">
                  {productDetails.images.length > 0 ? (
                    <>
                      <div className="position-relative">
                        <img
                          src={productDetails.images[currentIndex].url}
                          className="img-fluid"
                          alt={`${productDetails.name} - ${currentIndex + 1}`}
                        />

                        <div className="position-absolute top-50 start-0 translate-middle-y">
                          <button
                            className="btn btn-sm btn-light rounded-circle"
                            onClick={prevSlide}
                          >
                            <i className="bi bi-chevron-left"></i>
                          </button>
                        </div>

                        <div className="position-absolute top-50 end-0 translate-middle-y">
                          <button
                            className="btn btn-sm btn-light rounded-circle"
                            onClick={nextSlide}
                          >
                            <i className="bi bi-chevron-right"></i>
                          </button>
                        </div>
                      </div>

                      <div className="mt-2 px-2 pb-2">
                        <div className="d-flex justify-content-center">
                          <small>
                            {currentIndex + 1} / {productDetails.images.length}
                          </small>
                        </div>
                      </div>
                    </>
                  ) : (
                    <div className="text-center p-3">
                      <i className="bi bi-image text-muted fs-1"></i>
                      <p className="mt-2 mb-0">No images available</p>
                    </div>
                  )}
                </div>
              </div>
            </div>

            {/* Right column - Details */}
            <div className="col-md-8">
              <div className="row g-3">
                {/* Categories */}
                <div className="col-12">
                  <div className="card">
                    <div className="card-header py-2">
                      <h6 className="mb-0">Categories</h6>
                    </div>
                    <div className="card-body py-2">
                      {productDetails.categories.length > 0 ? (
                        productDetails.categories.map((category) => (
                          <span
                            key={category.id}
                            className="badge bg-info me-1"
                          >
                            {category.name}
                          </span>
                        ))
                      ) : (
                        <span className="text-muted">
                          No categories assigned
                        </span>
                      )}
                    </div>
                  </div>
                </div>

                {/* Description */}
                <div className="col-12">
                  <div className="card">
                    <div className="card-header py-2 d-flex justify-content-between align-items-center">
                      <h6 className="mb-0">Description</h6>
                      {/*
                      <button className="btn btn-sm btn-outline-secondary">Edit</button>
                      */}
                    </div>
                    <div
                      className="card-body py-2 description-container"
                      style={{ maxHeight: "256px", overflowY: "auto" }}
                    >
                      <div
                        dangerouslySetInnerHTML={{
                          __html: productDetails.descriptionHtml,
                        }}
                      />
                    </div>
                  </div>
                </div>

                {/* Warranty */}
                <div className="col-12">
                  <div className="card">
                    <div className="card-header py-2 d-flex justify-content-between align-items-center">
                      <h6 className="mb-0">Warranty</h6>
                      {/*
                      <button className="btn btn-sm btn-outline-secondary">Edit</button>
                      */}
                    </div>
                    <div
                      className="card-body py-2 warranty-container"
                      style={{ maxHeight: "256px", overflowY: "auto" }}
                    >
                      <div
                        dangerouslySetInnerHTML={{
                          __html: productDetails.warrantyHtml,
                        }}
                      />
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Reviews - Full width */}
            <div className="col-12">
              <div className="card">
                <div className="card-header py-2 d-flex justify-content-between align-items-center">
                  <h6 className="mb-0">
                    Customer Reviews ({productDetails.reviews.length})
                  </h6>
                  <button className="btn btn-sm btn-outline-primary">
                    Manage Reviews
                  </button>
                </div>
                <div className="card-body p-0">
                  {productDetails.reviews.length === 0 ? (
                    <p className="text-center py-3 mb-0">No reviews yet.</p>
                  ) : (
                    <div className="table-responsive">
                      <table className="table table-hover table-striped mb-0">
                        <thead>
                          <tr>
                            <th>#</th>
                            <th>Customer</th>
                            <th>Rating</th>
                            <th>Title</th>
                            <th>Review</th>
                            <th></th>
                          </tr>
                        </thead>
                        <tbody>
                          {productDetails.reviews.map((review, index) => (
                            <tr key={review.id}>
                              <td>{index + 1}</td>
                              <td>
                                {review.userFirstName} {review.userLastName}
                              </td>
                              <td>
                                <div className="d-flex align-items-center">
                                  <span className="me-1">{review.rating}</span>
                                  <i className="bi bi-star-fill text-warning"></i>
                                </div>
                              </td>
                              {review.reviewTitle ? (
                                <td>{review.reviewTitle}</td>
                              ) : (
                                <td className="text-muted fst-italic">
                                  No title.
                                </td>
                              )}
                              {review.reviewText ? (
                                <td>{review.reviewText}</td>
                              ) : (
                                <td className="text-muted fst-italic">
                                  No review provided.
                                </td>
                              )}
                              <td>
                                <button className="btn btn-sm btn-outline-danger float-end">
                                  <i className="bi bi-trash"></i>
                                </button>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className="card-footer bg-light">
          <div className="d-flex justify-content-start">
            <button
              className="btn btn-sm btn-outline-secondary me-2"
              onClick={() => navigate("/products")}
            >
              <i className="bi bi-arrow-left"></i> Back
            </button>
            <button
              className="btn btn-sm btn-primary"
              onClick={() => navigate(`/products/edit/${productDetails.id}`)}
            >
              <i className="bi bi-pencil"></i> Edit
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ProductDetailsPage;
