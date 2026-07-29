import { useState } from 'react'
import { useMyOrders } from '../../hooks/useOrders'
import { LoadingSpinner } from '../common/LoadingSpinner'
import { ErrorAlert } from '../common/ErrorAlert'
import { formatCurrency } from '../../lib/currency'
import type { OrderStatus } from '../../api/types'

const PAGE_SIZE = 10

const statusStyles: Record<OrderStatus, string> = {
  PendingPayment: 'bg-amber-100 text-amber-800',
  Paid: 'bg-green-100 text-green-800',
  Cancelled: 'bg-slate-100 text-slate-600',
}

export function OrderHistoryPage() {
  const [pageNumber, setPageNumber] = useState(1)
  const [expanded, setExpanded] = useState<string | null>(null)
  const { data, isLoading, error } = useMyOrders(pageNumber, PAGE_SIZE)

  if (isLoading) return <LoadingSpinner />
  if (error) return <ErrorAlert error={error} />
  if (!data) return null

  return (
    <div className="mx-auto max-w-3xl">
      <h1 className="mb-6 text-2xl font-bold text-slate-900">My Orders</h1>

      {data.items.length === 0 ? (
        <p className="text-slate-500">You haven't placed any orders yet.</p>
      ) : (
        <div className="divide-y divide-slate-100 rounded-lg border border-slate-200 bg-white">
          {data.items.map((order) => {
            const isExpanded = expanded === order.id
            return (
              <div key={order.id}>
                <button
                  type="button"
                  onClick={() => setExpanded(isExpanded ? null : order.id)}
                  className="flex w-full items-center justify-between px-4 py-4 text-left"
                >
                  <div>
                    <div className="text-sm text-slate-500">
                      {new Date(order.createdAtUtc).toLocaleString('en-IN')}
                    </div>
                    <div className="mt-1 font-medium text-slate-900">{formatCurrency(order.totalAmount)}</div>
                  </div>
                  <span className={`rounded-full px-3 py-1 text-xs font-medium ${statusStyles[order.status]}`}>
                    {order.status}
                  </span>
                </button>
                {isExpanded && (
                  <div className="border-t border-slate-100 bg-slate-50 px-4 py-3">
                    {order.lines.map((line) => (
                      <div key={line.productId} className="flex justify-between py-1 text-sm text-slate-600">
                        <span>
                          {line.productName} × {line.quantity}
                        </span>
                        <span>{formatCurrency(line.lineTotal)}</span>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            )
          })}
        </div>
      )}

      <div className="mt-4 flex items-center justify-between text-sm text-slate-500">
        <span>
          Page {data.pageNumber} of {Math.max(data.totalPages, 1)} ({data.totalCount} total)
        </span>
        <div className="flex gap-2">
          <button
            type="button"
            disabled={pageNumber <= 1}
            onClick={() => setPageNumber((p) => p - 1)}
            className="rounded-md border border-slate-300 px-3 py-1.5 font-medium hover:bg-slate-100 disabled:opacity-50"
          >
            Previous
          </button>
          <button
            type="button"
            disabled={pageNumber >= data.totalPages}
            onClick={() => setPageNumber((p) => p + 1)}
            className="rounded-md border border-slate-300 px-3 py-1.5 font-medium hover:bg-slate-100 disabled:opacity-50"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  )
}
