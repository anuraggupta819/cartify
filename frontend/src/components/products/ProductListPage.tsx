import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useProducts } from '../../hooks/useProducts'
import { useCategories } from '../../hooks/useCategories'
import { LoadingSpinner } from '../common/LoadingSpinner'
import { ErrorAlert } from '../common/ErrorAlert'
import { DeleteProductButton } from './DeleteProductButton'
import { formatCurrency } from '../../lib/currency'

const PAGE_SIZE = 10

export function ProductListPage() {
  const [pageNumber, setPageNumber] = useState(1)
  const { data, isLoading, error } = useProducts(pageNumber, PAGE_SIZE)
  const { data: categories } = useCategories()

  const categoryName = (categoryId: string) =>
    categories?.find((c) => c.id === categoryId)?.name ?? '—'

  if (isLoading) return <LoadingSpinner />
  if (error) return <ErrorAlert error={error} />
  if (!data) return null

  return (
    <div>
      <h1 className="mb-6 text-2xl font-bold text-slate-900">Products</h1>

      {data.items.length === 0 ? (
        <p className="text-slate-500">No products yet. Create one to get started.</p>
      ) : (
        <div className="overflow-hidden rounded-lg border border-slate-200 bg-white">
          <table className="w-full text-left text-sm">
            <thead className="border-b border-slate-200 bg-slate-50 text-slate-600">
              <tr>
                <th className="px-4 py-3 font-medium">Name</th>
                <th className="px-4 py-3 font-medium">SKU</th>
                <th className="px-4 py-3 font-medium">Category</th>
                <th className="px-4 py-3 font-medium">Price</th>
                <th className="px-4 py-3 font-medium"></th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {data.items.map((product) => (
                <tr key={product.id}>
                  <td className="px-4 py-3 font-medium text-slate-900">{product.name}</td>
                  <td className="px-4 py-3 text-slate-500">{product.sku}</td>
                  <td className="px-4 py-3 text-slate-500">{categoryName(product.categoryId)}</td>
                  <td className="px-4 py-3 text-slate-500">{formatCurrency(product.price)}</td>
                  <td className="px-4 py-3">
                    <div className="flex justify-end gap-4">
                      <Link
                        to={`/products/${product.id}/edit`}
                        className="text-sm font-medium text-indigo-600 hover:text-indigo-700"
                      >
                        Edit
                      </Link>
                      <DeleteProductButton productId={product.id} productName={product.name} />
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <div className="mt-4 flex items-center justify-between text-sm text-slate-500">
        <span>
          Page {data.pageNumber} of {Math.max(data.totalPages, 1)} ({data.totalCount} total)
        </span>
        <div className="flex gap-2">
          <button
            type="button"
            disabled={pageNumber <= 1}
            onClick={() => setPageNumber((p) => p - 1)}
            className="rounded-md border border-slate-300 px-3 py-1.5 font-medium hover:bg-slate-100 disabled:opacity-50"
          >
            Previous
          </button>
          <button
            type="button"
            disabled={pageNumber >= data.totalPages}
            onClick={() => setPageNumber((p) => p + 1)}
            className="rounded-md border border-slate-300 px-3 py-1.5 font-medium hover:bg-slate-100 disabled:opacity-50"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  )
}
