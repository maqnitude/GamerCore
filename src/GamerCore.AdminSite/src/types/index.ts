export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
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

export interface Category {
  categoryId: number;
  name: string;
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