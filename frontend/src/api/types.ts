export interface ProductDto {
  id: string
  name: string
  description: string
  sku: string
  price: number
  categoryId: string
  createdAtUtc: string
}

export interface CategoryDto {
  id: string
  name: string
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}

export interface CreateProductRequest {
  name: string
  description: string
  sku: string
  price: number
  categoryId: string
}

export interface UpdateProductRequest {
  name: string
  description: string
  price: number
  categoryId: string
}

export interface CreateCategoryRequest {
  name: string
}

export interface ProblemDetails {
  status?: number
  title?: string
  detail?: string
}

export class ApiError extends Error {
  status: number
  detail: string
  title?: string

  constructor(status: number, detail: string, title?: string) {
    super(detail)
    this.name = 'ApiError'
    this.status = status
    this.detail = detail
    this.title = title
  }
}
