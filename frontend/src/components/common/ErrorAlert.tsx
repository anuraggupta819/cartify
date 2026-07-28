import { ApiError } from '../../api/types'

function messageFor(error: unknown): string {
  if (error instanceof ApiError) {
    return error.detail
  }
  if (error instanceof Error) {
    return error.message
  }
  return 'Something went wrong.'
}

export function ErrorAlert({ error }: { error: unknown }) {
  if (!error) return null

  return (
    <div className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800">
      {messageFor(error)}
    </div>
  )
}
