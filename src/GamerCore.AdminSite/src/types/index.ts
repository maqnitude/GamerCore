/* Wrappers */

export interface ApiResponse<T> {
  data: T | null;
  error: ErrorDetail | null;
}

export interface ErrorDetail {
  code: number;
  message: string;
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
  id: string;
  name: string;
  description: string;
  productCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface Product {
  id: string;
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
  id: string;
  url: string;
  isPrimary: boolean;
}

export interface ProductReview {
  id: string;
  rating: number;
  reviewTitle: string | null;
  reviewText: string | null;
  userFirstName: string;
  userLastName: string;
}

export interface ProductDetails {
  id: string;
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
  id: string;
  name: string;
  description: string;
}

export interface CreateProductPayload {
  name: string;
  price: number;
  descriptionHtml: string;
  warrantyHtml: string;
  categoryIds: string[];
  primaryImageUrl: string;
  imageUrls: string[] | null;
}

export interface UpdateProductPayload {
  id: string;
  name: string;
  price: number;
  categoryIds: string[];
  descriptionHtml: string;
  warrantyHtml: string;
  primaryImageUrl: string;
  imageUrls: string[] | null;
}
