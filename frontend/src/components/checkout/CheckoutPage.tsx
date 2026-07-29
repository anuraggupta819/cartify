import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useCart } from '../../hooks/useCart'
import { useAuth } from '../../hooks/useAuth'
import { useCreateOrder } from '../../hooks/useOrders'
import { paymentsApi } from '../../api/payments'
import { loadRazorpayCheckout } from '../../lib/razorpay'
import { formatCurrency } from '../../lib/currency'
import { ErrorAlert } from '../common/ErrorAlert'

export function CheckoutPage() {
  const { items, totalPrice, clear } = useCart()
  const { user } = useAuth()
  const navigate = useNavigate()
  const createOrder = useCreateOrder()
  const [isProcessing, setIsProcessing] = useState(false)
  const [error, setError] = useState<unknown>(null)
  const [dismissed, setDismissed] = useState(false)

  async function handlePayNow() {
    setError(null)
    setDismissed(false)
    setIsProcessing(true)

    try {
      const order = await createOrder.mutateAsync({
        lines: items.map((item) => ({ productId: item.productId, quantity: item.quantity })),
      })

      const razorpayOrder = await paymentsApi.createRazorpayOrder({ orderId: order.id })

      await loadRazorpayCheckout()

      new window.Razorpay({
        key: razorpayOrder.razorpayKeyId,
        amount: razorpayOrder.amountInPaise,
        currency: razorpayOrder.currency,
        order_id: razorpayOrder.razorpayOrderId,
        name: 'Cartify',
        description: `Order ${order.id}`,
        handler: async (response) => {
          try {
            const result = await paymentsApi.verify({
              razorpayOrderId: response.razorpay_order_id,
              razorpayPaymentId: response.razorpay_payment_id,
              razorpaySignature: response.razorpay_signature,
            })
            if (result.verified) {
              clear()
              navigate('/orders')
            } else {
              setError(new Error('Payment verification failed. Please contact support if you were charged.'))
            }
          } catch (verifyError) {
            setError(verifyError)
          } finally {
            setIsProcessing(false)
          }
        },
        modal: {
          ondismiss: () => {
            setIsProcessing(false)
            setDismissed(true)
          },
        },
        theme: { color: '#4f46e5' },
      }).open()
    } catch (checkoutError) {
      setError(checkoutError)
      setIsProcessing(false)
    }
  }

  if (items.length === 0) {
    return <p className="text-slate-500">Your cart is empty.</p>
  }

  return (
    <div className="mx-auto max-w-lg">
      <h1 className="mb-6 text-2xl font-bold text-slate-900">Checkout</h1>

      <div className="rounded-lg border border-slate-200 bg-white p-6">
        <div className="mb-4 text-sm text-slate-600">Paying as {user?.email}</div>

        <div className="divide-y divide-slate-100">
          {items.map((item) => (
            <div key={item.productId} className="flex justify-between py-2 text-sm">
              <span>
                {item.name} × {item.quantity}
              </span>
              <span className="font-medium">{formatCurrency(item.price * item.quantity)}</span>
            </div>
          ))}
        </div>

        <div className="mt-4 flex justify-between border-t border-slate-200 pt-4 text-lg font-semibold text-slate-900">
          <span>Total</span>
          <span>{formatCurrency(totalPrice)}</span>
        </div>

        {dismissed && (
          <p className="mt-4 text-sm text-amber-600">
            Payment window closed. Your order is reserved for a short while — try again when you're ready.
          </p>
        )}

        <ErrorAlert error={error} />

        <button
          type="button"
          disabled={isProcessing}
          onClick={handlePayNow}
          className="mt-6 w-full rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700 disabled:opacity-50"
        >
          {isProcessing ? 'Processing…' : 'Pay Now'}
        </button>
      </div>
    </div>
  )
}
