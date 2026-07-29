import { apiClient } from './client'
import type { SetStockQuantityRequest, StockDto } from './types'

export const stockApi = {
  getByProductId: (productId: string) => apiClient.get<StockDto>(`/stock/${productId}`),

  setQuantity: (productId: string, request: SetStockQuantityRequest) =>
    apiClient.put<void>(`/stock/${productId}`, request),
}
