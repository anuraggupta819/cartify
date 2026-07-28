import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { categoriesApi } from '../api/categories'
import type { CreateCategoryRequest } from '../api/types'

export function useCategories() {
  return useQuery({
    queryKey: ['categories'],
    queryFn: () => categoriesApi.getAll(),
  })
}

export function useCreateCategory() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (request: CreateCategoryRequest) => categoriesApi.create(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'] })
    },
  })
}
