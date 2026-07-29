export interface ProductDto {
  id: string
  name: string
  description: string
  sku: string
  price: number
  categoryId: string
  imageUrl: string | null
  createdAtUtc: string
}

export interface CategoryDto {
  id: string
  name: string
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}

export interface CreateProductRequest {
  name: string
  description: string
  sku: string
  price: number
  categoryId: string
  imageUrl: string | null
  initialStockQuantity: number
}

export interface UpdateProductRequest {
  name: string
  description: string
  price: number
  categoryId: string
  imageUrl: string | null
}

export interface CreateCategoryRequest {
  name: string
}

export type UserRole = 'Admin' | 'Customer'

export interface GoogleLoginRequest {
  idToken: string
}

export interface AdminLoginRequest {
  username: string
  password: string
}

export interface AuthResponse {
  token: string
  email: string
  name: string | null
  role: UserRole
}

export interface StockDto {
  productId: string
  quantity: number
  reserved: number
  available: number
}

export interface SetStockQuantityRequest {
  quantity: number
}

export type OrderStatus = 'PendingPayment' | 'Paid' | 'Cancelled'

export interface OrderLineDto {
  productId: string
  productName: string
  unitPrice: number
  quantity: number
  lineTotal: number
}

export interface OrderDto {
  id: string
  status: OrderStatus
  totalAmount: number
  createdAtUtc: string
  lines: OrderLineDto[]
}

export interface CreateOrderLineRequest {
  productId: string
  quantity: number
}

export interface CreateOrderRequest {
  lines: CreateOrderLineRequest[]
}

export interface CreateRazorpayOrderRequest {
  orderId: string
}

export interface RazorpayOrderResponse {
  razorpayOrderId: string
  razorpayKeyId: string
  amountInPaise: number
  currency: string
}

export interface VerifyPaymentRequest {
  razorpayOrderId: string
  razorpayPaymentId: string
  razorpaySignature: string
}

export interface ProblemDetails {
  status?: number
  title?: string
  detail?: string
}

export class ApiError extends Error {
  status: number
  detail: string
  title?: string

  constructor(status: number, detail: string, title?: string) {
    super(detail)
    this.name = 'ApiError'
    this.status = status
    this.detail = detail
    this.title = title
  }
}
