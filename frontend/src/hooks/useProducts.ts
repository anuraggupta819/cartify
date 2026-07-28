import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { productsApi } from '../api/products'
import type { CreateProductRequest, UpdateProductRequest } from '../api/types'

export function useProducts(pageNumber: number, pageSize: number) {
  return useQuery({
    queryKey: ['products', pageNumber, pageSize],
    queryFn: () => productsApi.getPaged(pageNumber, pageSize),
  })
}

export function useProduct(id: string | undefined) {
  return useQuery({
    queryKey: ['products', id],
    queryFn: () => productsApi.getById(id!),
    enabled: !!id,
  })
}

export function useCreateProduct() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (request: CreateProductRequest) => productsApi.create(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] })
    },
  })
}

export function useUpdateProduct(id: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (request: UpdateProductRequest) => productsApi.update(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] })
    },
  })
}

export function useDeleteProduct() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => productsApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] })
    },
  })
}
