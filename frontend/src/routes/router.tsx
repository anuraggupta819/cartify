import { createBrowserRouter } from 'react-router-dom'
import { AppShell } from '../components/layout/AppShell'
import { ProductListPage } from '../components/products/ProductListPage'
import { ProductCreatePage } from '../components/products/ProductCreatePage'
import { ProductEditPage } from '../components/products/ProductEditPage'
import { LoginPage } from '../components/auth/LoginPage'
import { AdminRoute } from '../components/auth/AdminRoute'

export const router = createBrowserRouter([
  {
    path: '/',
    element: <AppShell />,
    children: [
      { index: true, element: <ProductListPage /> },
      { path: 'login', element: <LoginPage /> },
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
    ],
  },
])
