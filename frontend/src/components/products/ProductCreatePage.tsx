import { useNavigate } from 'react-router-dom'
import { useCreateProduct } from '../../hooks/useProducts'
import { ProductForm, type ProductFormValues } from './ProductForm'

export function ProductCreatePage() {
  const navigate = useNavigate()
  const createProduct = useCreateProduct()

  async function handleSubmit(values: ProductFormValues) {
    try {
      await createProduct.mutateAsync(values)
      navigate('/')
    } catch {
      // surfaced via createProduct.error / ErrorAlert below
    }
  }

  return (
    <div className="mx-auto max-w-lg">
      <h1 className="mb-6 text-2xl font-bold text-slate-900">New Product</h1>
      <ProductForm
        mode="create"
        onSubmit={handleSubmit}
        isSubmitting={createProduct.isPending}
        error={createProduct.error}
      />
    </div>
  )
}
