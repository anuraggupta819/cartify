import { useState } from 'react'
import { useParams } from 'react-router-dom'
import { useProduct } from '../../hooks/useProducts'
import { useCategories } from '../../hooks/useCategories'
import { useStock } from '../../hooks/useStock'
import { useCart } from '../../hooks/useCart'
import { useTapPulse } from '../../hooks/useTapPulse'
import { LoadingSpinner } from '../common/LoadingSpinner'
import { ErrorAlert } from '../common/ErrorAlert'
import { formatCurrency } from '../../lib/currency'

export function ProductDetailPage() {
  const { id } = useParams<{ id: string }>()
  const { data: product, isLoading, error } = useProduct(id)
  const { data: categories } = useCategories()
  const { data: stock, isLoading: stockLoading } = useStock(id)
  const { addItem } = useCart()
  const { isPulsing, pulse } = useTapPulse()
  const [quantity, setQuantity] = useState(1)

  if (isLoading) return <LoadingSpinner />
  if (error) return <ErrorAlert error={error} />
  if (!product) return null

  const categoryName = categories?.find((c) => c.id === product.categoryId)?.name ?? '—'
  const available = stockLoading ? null : (stock?.available ?? 0)
  const outOfStock = available !== null && available <= 0

  return (
    <div className="mx-auto grid max-w-4xl grid-cols-1 gap-8 sm:grid-cols-2">
      <div className="aspect-square overflow-hidden rounded-lg border border-slate-200 bg-slate-100">
        {product.imageUrl ? (
          <img src={product.imageUrl} alt={product.name} className="h-full w-full object-cover" />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-slate-400">No image</div>
        )}
      </div>

      <div>
        <span className="text-xs font-medium text-slate-500">{categoryName}</span>
        <h1 className="mt-1 text-2xl font-bold text-slate-900">{product.name}</h1>
        <p className="mt-3 text-slate-600">{product.description}</p>

        <div className="mt-4 text-2xl font-semibold text-slate-900">{formatCurrency(product.price)}</div>

        <div className="mt-2 text-sm font-medium">
          {available === null ? null : outOfStock ? (
            <span className="text-red-600">Out of stock</span>
          ) : available <= 5 ? (
            <span className="text-amber-600">Only {available} left in stock</span>
          ) : (
            <span className="text-green-600">In stock</span>
          )}
        </div>

        <div className="mt-6 flex items-center gap-3">
          <input
            type="number"
            min="1"
            value={quantity}
            onChange={(e) => setQuantity(Math.max(1, Number(e.target.value)))}
            className="w-20 rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
          />
          <button
            type="button"
            disabled={outOfStock}
            onClick={() => {
              addItem(product, quantity)
              pulse()
            }}
            className={`rounded-md bg-indigo-600 px-6 py-2 text-sm font-medium text-white transition-transform duration-150 hover:bg-indigo-700 disabled:cursor-not-allowed disabled:opacity-50 ${isPulsing ? 'scale-90' : 'scale-100'}`}
          >
            Add to Cart
          </button>
        </div>
      </div>
    </div>
  )
}
