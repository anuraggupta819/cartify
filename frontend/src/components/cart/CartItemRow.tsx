import type { CartItem } from '../../hooks/useCart'
import { useTapPulse } from '../../hooks/useTapPulse'
import { formatCurrency } from '../../lib/currency'

interface CartItemRowProps {
  item: CartItem
  onUpdateQuantity: (productId: string, quantity: number) => void
  onRemove: (productId: string) => void
}

export function CartItemRow({ item, onUpdateQuantity, onRemove }: CartItemRowProps) {
  const { isPulsing, pulse } = useTapPulse()

  function handleQuantityChange(rawValue: string) {
    const quantity = Math.max(1, Number(rawValue))
    if (quantity > item.quantity) {
      pulse()
    }
    onUpdateQuantity(item.productId, quantity)
  }

  return (
    <div className="flex items-center gap-4 p-4">
      <div className="h-16 w-16 flex-shrink-0 overflow-hidden rounded-md bg-slate-100">
        {item.imageUrl ? <img src={item.imageUrl} alt={item.name} className="h-full w-full object-cover" /> : null}
      </div>
      <div className="flex-1">
        <div className="font-medium text-slate-900">{item.name}</div>
        <div className="text-sm text-slate-500">{formatCurrency(item.price)} each</div>
      </div>
      <input
        type="number"
        min="1"
        value={item.quantity}
        onChange={(e) => handleQuantityChange(e.target.value)}
        className={`w-16 rounded-md border border-slate-300 px-2 py-1 text-sm transition-transform duration-150 focus:border-indigo-500 focus:outline-none ${isPulsing ? 'scale-110' : 'scale-100'}`}
      />
      <div className="w-24 text-right font-medium text-slate-900">{formatCurrency(item.price * item.quantity)}</div>
      <button
        type="button"
        onClick={() => onRemove(item.productId)}
        className="text-sm font-medium text-red-600 hover:text-red-700"
      >
        Remove
      </button>
    </div>
  )
}
