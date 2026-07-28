import { createBrowserRouter } from 'react-router-dom'
import { AppShell } from '../components/layout/AppShell'
import { ProductListPage } from '../components/products/ProductListPage'
import { ProductCreatePage } from '../components/products/ProductCreatePage'
import { ProductEditPage } from '../components/products/ProductEditPage'

export const router = createBrowserRouter([
  {
    path: '/',
    element: <AppShell />,
    children: [
      { index: true, element: <ProductListPage /> },
      { path: 'products/new', element: <ProductCreatePage /> },
      { path: 'products/:id/edit', element: <ProductEditPage /> },
    ],
  },
])
