import { useNavigate, useParams } from 'react-router-dom'
import { useProduct, useUpdateProduct } from '../../hooks/useProducts'
import { LoadingSpinner } from '../common/LoadingSpinner'
import { ErrorAlert } from '../common/ErrorAlert'
import { ProductForm, type ProductFormValues } from './ProductForm'

export function ProductEditPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { data: product, isLoading, error: loadError } = useProduct(id)
  const updateProduct = useUpdateProduct(id!)

  async function handleSubmit(values: ProductFormValues) {
    try {
      await updateProduct.mutateAsync({
        name: values.name,
        description: values.description,
        price: values.price,
        categoryId: values.categoryId,
      })
      navigate('/')
    } catch {
      // surfaced via updateProduct.error / ErrorAlert below
    }
  }

  if (isLoading) return <LoadingSpinner />
  if (loadError) return <ErrorAlert error={loadError} />
  if (!product) return null

  return (
    <div className="mx-auto max-w-lg">
      <h1 className="mb-6 text-2xl font-bold text-slate-900">Edit Product</h1>
      <ProductForm
        mode="edit"
        initialValues={product}
        onSubmit={handleSubmit}
        isSubmitting={updateProduct.isPending}
        error={updateProduct.error}
      />
    </div>
  )
}
