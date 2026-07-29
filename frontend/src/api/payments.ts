import { apiClient } from './client'
import type { CreateRazorpayOrderRequest, RazorpayOrderResponse, VerifyPaymentRequest } from './types'

export const paymentsApi = {
  createRazorpayOrder: (request: CreateRazorpayOrderRequest) =>
    apiClient.post<RazorpayOrderResponse>('/payments/razorpay-order', request),

  verify: (request: VerifyPaymentRequest) =>
    apiClient.post<{ verified: boolean }>('/payments/verify', request),
}
