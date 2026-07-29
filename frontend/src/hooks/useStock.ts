import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { stockApi } from '../api/stock'
import type { SetStockQuantityRequest } from '../api/types'

export function useStock(productId: string | undefined) {
  return useQuery({
    queryKey: ['stock', productId],
    queryFn: () => stockApi.getByProductId(productId!),
    enabled: !!productId,
    retry: false,
  })
}

export function useSetStockQuantity(productId: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (request: SetStockQuantityRequest) => stockApi.setQuantity(productId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['stock', productId] })
    },
  })
}
