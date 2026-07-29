import { apiClient } from './client'
import type { CreateOrderRequest, OrderDto, PagedResult } from './types'

export const ordersApi = {
  create: (request: CreateOrderRequest) => apiClient.post<OrderDto>('/orders', request),

  getMine: (pageNumber: number, pageSize: number) =>
    apiClient.get<PagedResult<OrderDto>>(`/orders/mine?pageNumber=${pageNumber}&pageSize=${pageSize}`),

  getById: (id: string) => apiClient.get<OrderDto>(`/orders/${id}`),
}
