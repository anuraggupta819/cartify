import { apiClient } from './client'
import type { CategoryDto, CreateCategoryRequest } from './types'

export const categoriesApi = {
  getAll: () => apiClient.get<CategoryDto[]>('/categories'),

  create: (request: CreateCategoryRequest) => apiClient.post<CategoryDto>('/categories', request),
}
