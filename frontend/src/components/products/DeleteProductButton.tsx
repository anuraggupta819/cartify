import { useState } from 'react'
import { useDeleteProduct } from '../../hooks/useProducts'
import { ConfirmDialog } from '../common/ConfirmDialog'
import { ErrorAlert } from '../common/ErrorAlert'

export function DeleteProductButton({ productId, productName }: { productId: string; productName: string }) {
  const [confirmOpen, setConfirmOpen] = useState(false)
  const deleteProduct = useDeleteProduct()

  function handleConfirm() {
    deleteProduct.mutate(productId)
    setConfirmOpen(false)
  }

  return (
    // A single wrapping element (not a bare Fragment) so this stays one flex item in the
    // parent row, and the error stacks below the button instead of becoming its own
    // sibling flex item squeezed in next to "Edit"/"Delete".
    <div>
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
      {deleteProduct.isError && (
        <div className="mt-2 max-w-xs">
          <ErrorAlert error={deleteProduct.error} />
        </div>
      )}
    </div>
  )
}
