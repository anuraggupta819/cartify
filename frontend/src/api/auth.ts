import { apiClient } from './client'
import type { AdminLoginRequest, AuthResponse, GoogleLoginRequest } from './types'

export const authApi = {
  loginWithGoogle: (request: GoogleLoginRequest) => apiClient.post<AuthResponse>('/auth/google', request),
  loginAsAdmin: (request: AdminLoginRequest) => apiClient.post<AuthResponse>('/auth/admin-login', request),
}
