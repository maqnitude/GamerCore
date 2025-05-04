/* Wrappers */

export interface ApiResponse {
  succeeded: boolean;
  message: string;
  data: unknown | null;
  errors: string[];
}

export interface PaginatedList<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
}

/* DTOs */

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
}

export interface Category {
  categoryId: number;
  name: string;
  description: string;
  productCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface Product {
  productId: number;
  name: string;
  price: number;
  categories: Category[];
  thumbnailUrl: string;
  averageRating: number;
  reviewCount: number;
  createdAt: string;
  updatedAt: string;
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
  createdAt: string;
  updatedAt: string;
}

/* Payloads */

export interface LoginPayload {
  email: string;
  password: string;
  rememberMe: boolean;
}

export interface CreateCategoryPayload {
  name: string;
  description: string;
}

export interface UpdateCategoryPayload {
  categoryId: number;
  name: string;
  description: string;
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