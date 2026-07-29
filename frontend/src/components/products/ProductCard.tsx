import { Link } from 'react-router-dom'
import type { ProductDto } from '../../api/types'
import { useStock } from '../../hooks/useStock'
import { useCart } from '../../hooks/useCart'
import { useAuth } from '../../hooks/useAuth'
import { useTapPulse } from '../../hooks/useTapPulse'
import { formatCurrency } from '../../lib/currency'
import { DeleteProductButton } from './DeleteProductButton'

export function ProductCard({ product, categoryName }: { product: ProductDto; categoryName: string }) {
  const { data: stock, isLoading: stockLoading } = useStock(product.id)
  const { addItem } = useCart()
  const { isAdmin } = useAuth()
  const { isPulsing, pulse } = useTapPulse()

  // No stock row (404) is treated the same as zero available, not "unknown" —
  // a product with no inventory record has nothing to sell.
  const available = stockLoading ? null : (stock?.available ?? 0)
  const outOfStock = available !== null && available <= 0

  return (
    <div className="flex flex-col overflow-hidden rounded-lg border border-slate-200 bg-white">
      <Link to={`/products/${product.id}`} className="block aspect-square bg-slate-100">
        {product.imageUrl ? (
          <img src={product.imageUrl} alt={product.name} className="h-full w-full object-cover" />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-sm text-slate-400">No image</div>
        )}
      </Link>
      <div className="flex flex-1 flex-col p-4">
        <Link to={`/products/${product.id}`} className="font-medium text-slate-900 hover:text-indigo-600">
          {product.name}
        </Link>
        <span className="text-xs text-slate-500">{categoryName}</span>

        <div className="mt-1 text-xs font-medium">
          {available === null ? null : outOfStock ? (
            <span className="text-red-600">Out of stock</span>
          ) : available <= 5 ? (
            <span className="text-amber-600">Only {available} left</span>
          ) : (
            <span className="text-green-600">In stock</span>
          )}
        </div>

        <div className="mt-auto flex items-center justify-between pt-3">
          <span className="font-semibold text-slate-900">{formatCurrency(product.price)}</span>
          <button
            type="button"
            disabled={outOfStock}
            onClick={() => {
              addItem(product)
              pulse()
            }}
            className={`rounded-md bg-indigo-600 px-3 py-1.5 text-sm font-medium text-white transition-transform duration-150 hover:bg-indigo-700 disabled:cursor-not-allowed disabled:opacity-50 ${isPulsing ? 'scale-90' : 'scale-100'}`}
          >
            Add to Cart
          </button>
        </div>

        {isAdmin && (
          <div className="mt-3 flex justify-end gap-4 border-t border-slate-100 pt-3">
            <Link to={`/products/${product.id}/edit`} className="text-sm font-medium text-indigo-600 hover:text-indigo-700">
              Edit
            </Link>
            <DeleteProductButton productId={product.id} productName={product.name} />
          </div>
        )}
      </div>
    </div>
  )
}
