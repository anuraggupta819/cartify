import { useProducts } from '../../hooks/useProducts'
import { LoadingSpinner } from '../common/LoadingSpinner'
import { ErrorAlert } from '../common/ErrorAlert'
import { StockRow } from './StockRow'

const PAGE_SIZE = 100

export function StockManagementPage() {
  const { data, isLoading, error } = useProducts(1, PAGE_SIZE)

  if (isLoading) return <LoadingSpinner />
  if (error) return <ErrorAlert error={error} />
  if (!data) return null

  return (
    <div>
      <h1 className="mb-6 text-2xl font-bold text-slate-900">Manage Stock</h1>

      {data.items.length === 0 ? (
        <p className="text-slate-500">No products yet.</p>
      ) : (
        <div className="overflow-hidden rounded-lg border border-slate-200 bg-white">
          <table className="w-full text-left text-sm">
            <thead className="border-b border-slate-200 bg-slate-50 text-slate-600">
              <tr>
                <th className="px-4 py-3 font-medium">Name</th>
                <th className="px-4 py-3 font-medium">SKU</th>
                <th className="px-4 py-3 font-medium">Reserved</th>
                <th className="px-4 py-3 font-medium">On-hand Quantity</th>
                <th className="px-4 py-3 font-medium"></th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {data.items.map((product) => (
                <StockRow key={product.id} product={product} />
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
