import { createBrowserRouter } from 'react-router-dom'
import { AppShell } from '../components/layout/AppShell'
import { ProductListPage } from '../components/products/ProductListPage'
import { ProductDetailPage } from '../components/products/ProductDetailPage'
import { ProductCreatePage } from '../components/products/ProductCreatePage'
import { ProductEditPage } from '../components/products/ProductEditPage'
import { LoginPage } from '../components/auth/LoginPage'
import { AdminRoute } from '../components/auth/AdminRoute'
import { RequireAuthRoute } from '../components/auth/RequireAuthRoute'
import { CartPage } from '../components/cart/CartPage'
import { CheckoutPage } from '../components/checkout/CheckoutPage'
import { OrderHistoryPage } from '../components/orders/OrderHistoryPage'
import { StockManagementPage } from '../components/admin/StockManagementPage'

export const router = createBrowserRouter([
  {
    path: '/',
    element: <AppShell />,
    children: [
      { index: true, element: <ProductListPage /> },
      { path: 'login', element: <LoginPage /> },
      { path: 'products/:id', element: <ProductDetailPage /> },
      { path: 'cart', element: <CartPage /> },
      {
        path: 'checkout',
        element: (
          <RequireAuthRoute>
            <CheckoutPage />
          </RequireAuthRoute>
        ),
      },
      {
        path: 'orders',
        element: (
          <RequireAuthRoute>
            <OrderHistoryPage />
          </RequireAuthRoute>
        ),
      },
      {
        path: 'products/new',
        element: (
          <AdminRoute>
            <ProductCreatePage />
          </AdminRoute>
        ),
      },
      {
        path: 'products/:id/edit',
        element: (
          <AdminRoute>
            <ProductEditPage />
          </AdminRoute>
        ),
      },
      {
        path: 'admin/stock',
        element: (
          <AdminRoute>
            <StockManagementPage />
          </AdminRoute>
        ),
      },
    ],
  },
])
