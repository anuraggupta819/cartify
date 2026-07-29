import { createContext, useContext, useEffect, useState, type ReactNode } from 'react'
import type { ProductDto } from '../api/types'

export interface CartItem {
  productId: string
  name: string
  price: number
  imageUrl: string | null
  quantity: number
}

interface CartContextValue {
  items: CartItem[]
  addItem: (product: ProductDto, quantity?: number) => void
  removeItem: (productId: string) => void
  updateQuantity: (productId: string, quantity: number) => void
  clear: () => void
  totalItems: number
  totalPrice: number
}

const STORAGE_KEY = 'cartify.cart'

const CartContext = createContext<CartContextValue | undefined>(undefined)

function readPersisted(): CartItem[] {
  const raw = localStorage.getItem(STORAGE_KEY)
  if (!raw) return []
  try {
    return JSON.parse(raw) as CartItem[]
  } catch {
    return []
  }
}

export function CartProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<CartItem[]>(() => readPersisted())

  useEffect(() => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(items))
  }, [items])

  function addItem(product: ProductDto, quantity = 1) {
    setItems((current) => {
      const existing = current.find((item) => item.productId === product.id)
      if (existing) {
        return current.map((item) =>
          item.productId === product.id ? { ...item, quantity: item.quantity + quantity } : item,
        )
      }
      return [
        ...current,
        { productId: product.id, name: product.name, price: product.price, imageUrl: product.imageUrl, quantity },
      ]
    })
  }

  function removeItem(productId: string) {
    setItems((current) => current.filter((item) => item.productId !== productId))
  }

  function updateQuantity(productId: string, quantity: number) {
    if (quantity <= 0) {
      removeItem(productId)
      return
    }
    setItems((current) => current.map((item) => (item.productId === productId ? { ...item, quantity } : item)))
  }

  function clear() {
    setItems([])
  }

  const totalItems = items.reduce((sum, item) => sum + item.quantity, 0)
  const totalPrice = items.reduce((sum, item) => sum + item.price * item.quantity, 0)

  const value: CartContextValue = { items, addItem, removeItem, updateQuantity, clear, totalItems, totalPrice }

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>
}

export function useCart(): CartContextValue {
  const context = useContext(CartContext)
  if (!context) {
    throw new Error('useCart must be used within a CartProvider')
  }
  return context
}
