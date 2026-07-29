import { Link, Outlet } from 'react-router-dom'
import { useAuth } from '../../hooks/useAuth'
import { useCart } from '../../hooks/useCart'

export function AppShell() {
  const { user, isAdmin, logout } = useAuth()
  const { totalItems } = useCart()

  return (
    <div className="min-h-screen bg-slate-50">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-6 py-4">
          <Link to="/" className="text-xl font-bold text-indigo-600">
            Cartify
          </Link>
          <nav className="flex items-center gap-4">
            {isAdmin && (
              <>
                <Link
                  to="/admin/stock"
                  className="text-sm font-medium text-slate-600 hover:text-slate-900"
                >
                  Manage Stock
                </Link>
                <Link
                  to="/products/new"
                  className="rounded-md bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700"
                >
                  New Product
                </Link>
              </>
            )}
            {user && (
              <Link to="/orders" className="text-sm font-medium text-slate-600 hover:text-slate-900">
                My Orders
              </Link>
            )}
            <Link to="/cart" className="text-sm font-medium text-slate-600 hover:text-slate-900">
              Cart{totalItems > 0 ? ` (${totalItems})` : ''}
            </Link>
            {user ? (
              <div className="flex items-center gap-3">
                <span className="text-sm text-slate-600">
                  {user.name ?? user.email} <span className="text-slate-400">({user.role})</span>
                </span>
                <button
                  type="button"
                  onClick={logout}
                  className="text-sm font-medium text-slate-600 hover:text-slate-900"
                >
                  Sign out
                </button>
              </div>
            ) : (
              <Link to="/login" className="text-sm font-medium text-indigo-600 hover:text-indigo-700">
                Sign in
              </Link>
            )}
          </nav>
        </div>
      </header>
      <main className="mx-auto max-w-5xl px-6 py-8">
        <Outlet />
      </main>
    </div>
  )
}
