import { Link, useNavigate } from 'react-router-dom'
import { useCart } from '../../hooks/useCart'
import { formatCurrency } from '../../lib/currency'

export function CartPage() {
  const { items, removeItem, updateQuantity, totalPrice } = useCart()
  const navigate = useNavigate()

  if (items.length === 0) {
    return (
      <div>
        <h1 className="mb-4 text-2xl font-bold text-slate-900">Your Cart</h1>
        <p className="text-slate-500">
          Your cart is empty.{' '}
          <Link to="/" className="font-medium text-indigo-600 hover:text-indigo-700">
            Browse products
          </Link>
          .
        </p>
      </div>
    )
  }

  return (
    <div className="mx-auto max-w-2xl">
      <h1 className="mb-6 text-2xl font-bold text-slate-900">Your Cart</h1>

      <div className="divide-y divide-slate-100 rounded-lg border border-slate-200 bg-white">
        {items.map((item) => (
          <div key={item.productId} className="flex items-center gap-4 p-4">
            <div className="h-16 w-16 flex-shrink-0 overflow-hidden rounded-md bg-slate-100">
              {item.imageUrl ? (
                <img src={item.imageUrl} alt={item.name} className="h-full w-full object-cover" />
              ) : null}
            </div>
            <div className="flex-1">
              <div className="font-medium text-slate-900">{item.name}</div>
              <div className="text-sm text-slate-500">{formatCurrency(item.price)} each</div>
            </div>
            <input
              type="number"
              min="1"
              value={item.quantity}
              onChange={(e) => updateQuantity(item.productId, Math.max(1, Number(e.target.value)))}
              className="w-16 rounded-md border border-slate-300 px-2 py-1 text-sm focus:border-indigo-500 focus:outline-none"
            />
            <div className="w-24 text-right font-medium text-slate-900">
              {formatCurrency(item.price * item.quantity)}
            </div>
            <button
              type="button"
              onClick={() => removeItem(item.productId)}
              className="text-sm font-medium text-red-600 hover:text-red-700"
            >
              Remove
            </button>
          </div>
        ))}
      </div>

      <div className="mt-6 flex items-center justify-between">
        <span className="text-lg font-semibold text-slate-900">Total: {formatCurrency(totalPrice)}</span>
        <button
          type="button"
          onClick={() => navigate('/checkout')}
          className="rounded-md bg-indigo-600 px-6 py-2 text-sm font-medium text-white hover:bg-indigo-700"
        >
          Proceed to Checkout
        </button>
      </div>
    </div>
  )
}
