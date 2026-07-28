import { useState } from 'react'
import { useDeleteProduct } from '../../hooks/useProducts'
import { ConfirmDialog } from '../common/ConfirmDialog'

export function DeleteProductButton({ productId, productName }: { productId: string; productName: string }) {
  const [confirmOpen, setConfirmOpen] = useState(false)
  const deleteProduct = useDeleteProduct()

  function handleConfirm() {
    deleteProduct.mutate(productId)
    setConfirmOpen(false)
  }

  return (
    <>
      <button
        type="button"
        onClick={() => setConfirmOpen(true)}
        className="text-sm font-medium text-red-600 hover:text-red-700"
      >
        Delete
      </button>
      <ConfirmDialog
        open={confirmOpen}
        title="Delete product"
        message={`Are you sure you want to delete "${productName}"? This can't be undone.`}
        confirmLabel="Delete"
        onConfirm={handleConfirm}
        onCancel={() => setConfirmOpen(false)}
      />
    </>
  )
}
