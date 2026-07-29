import { useState } from 'react'
import type { ProductDto } from '../../api/types'
import { useSetStockQuantity, useStock } from '../../hooks/useStock'

export function StockRow({ product }: { product: ProductDto }) {
  const { data: stock, isLoading } = useStock(product.id)
  const setQuantity = useSetStockQuantity(product.id)
  const [value, setValue] = useState<string | null>(null)

  const currentQuantity = value ?? stock?.quantity?.toString() ?? '0'

  function handleSave() {
    setQuantity.mutate({ quantity: Number(currentQuantity) }, { onSuccess: () => setValue(null) })
  }

  return (
    <tr>
      <td className="px-4 py-3 font-medium text-slate-900">{product.name}</td>
      <td className="px-4 py-3 text-slate-500">{product.sku}</td>
      <td className="px-4 py-3 text-slate-500">{isLoading ? '…' : (stock?.reserved ?? 0)}</td>
      <td className="px-4 py-3">
        <input
          type="number"
          min="0"
          value={currentQuantity}
          onChange={(e) => setValue(e.target.value)}
          className="w-24 rounded-md border border-slate-300 px-2 py-1 text-sm focus:border-indigo-500 focus:outline-none"
        />
      </td>
      <td className="px-4 py-3">
        <button
          type="button"
          disabled={setQuantity.isPending}
          onClick={handleSave}
          className="rounded-md bg-indigo-600 px-3 py-1.5 text-sm font-medium text-white hover:bg-indigo-700 disabled:opacity-50"
        >
          {setQuantity.isPending ? 'Saving…' : 'Save'}
        </button>
      </td>
    </tr>
  )
}
