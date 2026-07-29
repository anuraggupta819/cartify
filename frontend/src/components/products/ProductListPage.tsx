import { useState } from 'react'
import { useProducts } from '../../hooks/useProducts'
import { useCategories } from '../../hooks/useCategories'
import { LoadingSpinner } from '../common/LoadingSpinner'
import { ErrorAlert } from '../common/ErrorAlert'
import { ProductCard } from './ProductCard'

const PAGE_SIZE = 12

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
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {data.items.map((product) => (
            <ProductCard key={product.id} product={product} categoryName={categoryName(product.categoryId)} />
          ))}
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
