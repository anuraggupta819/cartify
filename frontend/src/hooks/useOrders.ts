import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { ordersApi } from '../api/orders'
import type { CreateOrderRequest } from '../api/types'

export function useMyOrders(pageNumber: number, pageSize: number) {
  return useQuery({
    queryKey: ['orders', 'mine', pageNumber, pageSize],
    queryFn: () => ordersApi.getMine(pageNumber, pageSize),
  })
}

export function useOrder(id: string | undefined) {
  return useQuery({
    queryKey: ['orders', id],
    queryFn: () => ordersApi.getById(id!),
    enabled: !!id,
  })
}

export function useCreateOrder() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (request: CreateOrderRequest) => ordersApi.create(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] })
    },
  })
}
