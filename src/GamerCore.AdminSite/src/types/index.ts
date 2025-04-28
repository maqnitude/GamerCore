export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
}

export interface Category {
  categoryId: number;
  name: string;
}

export interface Product {
  productId: number;
  name: string;
  price: number;
  categories: Category[];
  thumbnailUrl: string;
  averageRating: number;
  reviewCount: number;
}

export interface ProductImage {
  productImageId: number;
  url: string;
  isPrimary: boolean;
}

export interface ProductReview {
  productReviewId: number;
  rating: number;
  reviewText: string | null;
}

export interface ProductDetails {
  productId: number;
  name: string;
  price: number;
  categories: Category[];
  descriptionHtml: string;
  warrantyHtml: string;
  images: ProductImage[];
  averageRating: number;
  reviewCount: number;
  reviews: ProductReview[];
}

export interface CreateProductPayload {
  name: string;
  price: number;
  descriptionHtml: string;
  warrantyHtml: string;
  categoryIds: number[];
  primaryImageUrl: string;
  imageUrls: string[] | null;
}

export interface UpdateProductPayload {
  productId: number;
  name: string;
  price: number;
  categoryIds: number[];
  descriptionHtml: string;
  warrantyHtml: string;
  primaryImageUrl: string;
  imageUrls: string[] | null;
}