import { Link, useNavigate } from 'react-router-dom'
import { useCart } from '../../hooks/useCart'
import { formatCurrency } from '../../lib/currency'
import { CartItemRow } from './CartItemRow'

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
          <CartItemRow key={item.productId} item={item} onUpdateQuantity={updateQuantity} onRemove={removeItem} />
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
