import { useState } from 'react'
import { useCreateCategory } from '../../hooks/useCategories'
import { ErrorAlert } from '../common/ErrorAlert'

export function CategoryQuickCreate({ onCreated }: { onCreated: (categoryId: string) => void }) {
  const [open, setOpen] = useState(false)
  const [name, setName] = useState('')
  const createCategory = useCreateCategory()

  // Deliberately not a <form>: this renders inside ProductForm's <form>,
  // and nested <form> elements are invalid HTML (the browser silently drops
  // the inner one, so no onSubmit ever fires — falls back to a native
  // full-page GET). Plain button + Enter-to-submit avoids that entirely.
  async function submit() {
    if (!name.trim() || createCategory.isPending) return
    const category = await createCategory.mutateAsync({ name })
    setName('')
    setOpen(false)
    onCreated(category.id)
  }

  if (!open) {
    return (
      <button
        type="button"
        onClick={() => setOpen(true)}
        className="text-sm font-medium text-indigo-600 hover:text-indigo-700"
      >
        + New category
      </button>
    )
  }

  return (
    <div className="flex items-start gap-2">
      <div className="flex-1">
        <input
          autoFocus
          value={name}
          onChange={(e) => setName(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === 'Enter') {
              e.preventDefault()
              void submit()
            }
          }}
          placeholder="Category name"
          className="w-full rounded-md border border-slate-300 px-3 py-1.5 text-sm focus:border-indigo-500 focus:outline-none"
        />
        {createCategory.isError && <ErrorAlert error={createCategory.error} />}
      </div>
      <button
        type="button"
        onClick={() => void submit()}
        disabled={createCategory.isPending || !name.trim()}
        className="rounded-md bg-indigo-600 px-3 py-1.5 text-sm font-medium text-white hover:bg-indigo-700 disabled:opacity-50"
      >
        Add
      </button>
      <button
        type="button"
        onClick={() => setOpen(false)}
        className="rounded-md px-3 py-1.5 text-sm font-medium text-slate-600 hover:bg-slate-100"
      >
        Cancel
      </button>
    </div>
  )
}
