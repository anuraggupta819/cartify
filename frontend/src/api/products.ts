import { apiClient } from './client'
import type { CreateProductRequest, PagedResult, ProductDto, UpdateProductRequest } from './types'

export const productsApi = {
  getPaged: (pageNumber: number, pageSize: number) =>
    apiClient.get<PagedResult<ProductDto>>(`/products?pageNumber=${pageNumber}&pageSize=${pageSize}`),

  getById: (id: string) => apiClient.get<ProductDto>(`/products/${id}`),

  create: (request: CreateProductRequest) => apiClient.post<ProductDto>('/products', request),

  update: (id: string, request: UpdateProductRequest) =>
    apiClient.put<ProductDto>(`/products/${id}`, request),

  delete: (id: string) => apiClient.delete<void>(`/products/${id}`),
}
