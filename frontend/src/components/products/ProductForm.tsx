import { useState } from 'react'
import { useCategories } from '../../hooks/useCategories'
import { CategoryQuickCreate } from '../categories/CategoryQuickCreate'
import { ErrorAlert } from '../common/ErrorAlert'

export interface ProductFormValues {
  name: string
  description: string
  sku: string
  price: number
  categoryId: string
  imageUrl: string | null
  initialStockQuantity: number
}

interface ProductFormProps {
  mode: 'create' | 'edit'
  initialValues?: Partial<ProductFormValues>
  onSubmit: (values: ProductFormValues) => void
  isSubmitting: boolean
  error?: unknown
}

export function ProductForm({ mode, initialValues, onSubmit, isSubmitting, error }: ProductFormProps) {
  const [name, setName] = useState(initialValues?.name ?? '')
  const [description, setDescription] = useState(initialValues?.description ?? '')
  const [sku, setSku] = useState(initialValues?.sku ?? '')
  const [price, setPrice] = useState(initialValues?.price?.toString() ?? '')
  const [categoryId, setCategoryId] = useState(initialValues?.categoryId ?? '')
  const [imageUrl, setImageUrl] = useState(initialValues?.imageUrl ?? '')
  const [initialStockQuantity, setInitialStockQuantity] = useState(
    initialValues?.initialStockQuantity?.toString() ?? '0',
  )

  const { data: categories, isLoading: categoriesLoading } = useCategories()

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    onSubmit({
      name,
      description,
      sku,
      price: Number(price),
      categoryId,
      imageUrl: imageUrl.trim() === '' ? null : imageUrl.trim(),
      initialStockQuantity: Number(initialStockQuantity),
    })
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-5 rounded-lg border border-slate-200 bg-white p-6">
      <div>
        <label className="block text-sm font-medium text-slate-700">Name</label>
        <input
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
          className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
        />
      </div>

      <div>
        <label className="block text-sm font-medium text-slate-700">Description</label>
        <textarea
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          rows={3}
          className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
        />
      </div>

      <div>
        <label className="block text-sm font-medium text-slate-700">
          SKU {mode === 'edit' && <span className="text-slate-400">(not editable)</span>}
        </label>
        <input
          value={sku}
          onChange={(e) => setSku(e.target.value)}
          required={mode === 'create'}
          disabled={mode === 'edit'}
          className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none disabled:bg-slate-100 disabled:text-slate-500"
        />
      </div>

      <div>
        <label className="block text-sm font-medium text-slate-700">Price (₹)</label>
        <input
          type="number"
          step="0.01"
          value={price}
          onChange={(e) => setPrice(e.target.value)}
          required
          className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
        />
      </div>

      <div>
        <label className="block text-sm font-medium text-slate-700">Image URL</label>
        <input
          type="url"
          value={imageUrl}
          onChange={(e) => setImageUrl(e.target.value)}
          placeholder="https://example.com/image.jpg"
          className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
        />
        {imageUrl.trim() !== '' && (
          <img
            src={imageUrl}
            alt="Preview"
            className="mt-2 h-24 w-24 rounded-md border border-slate-200 object-cover"
            onError={(e) => {
              e.currentTarget.style.display = 'none'
            }}
          />
        )}
      </div>

      {mode === 'create' && (
        <div>
          <label className="block text-sm font-medium text-slate-700">Initial Stock Quantity</label>
          <input
            type="number"
            min="0"
            step="1"
            value={initialStockQuantity}
            onChange={(e) => setInitialStockQuantity(e.target.value)}
            required
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
          />
        </div>
      )}

      <div>
        <div className="flex items-center justify-between">
          <label className="block text-sm font-medium text-slate-700">Category</label>
          <CategoryQuickCreate onCreated={(id) => setCategoryId(id)} />
        </div>
        <select
          value={categoryId}
          onChange={(e) => setCategoryId(e.target.value)}
          required
          disabled={categoriesLoading}
          className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
        >
          <option value="" disabled>
            {categoriesLoading ? 'Loading categories…' : 'Select a category'}
          </option>
          {categories?.map((category) => (
            <option key={category.id} value={category.id}>
              {category.name}
            </option>
          ))}
        </select>
      </div>

      <ErrorAlert error={error} />

      <button
        type="submit"
        disabled={isSubmitting}
        className="w-full rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700 disabled:opacity-50"
      >
        {isSubmitting ? 'Saving…' : mode === 'create' ? 'Create Product' : 'Save Changes'}
      </button>
    </form>
  )
}
