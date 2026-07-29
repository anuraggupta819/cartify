import { useState } from 'react'
import { Link, Outlet } from 'react-router-dom'
import { useAuth } from '../../hooks/useAuth'
import { useCart } from '../../hooks/useCart'

export function AppShell() {
  const { user, isAdmin, logout } = useAuth()
  const { totalItems } = useCart()
  const [menuOpen, setMenuOpen] = useState(false)

  function closeMenu() {
    setMenuOpen(false)
  }

  const navLinks = (
    <>
      {isAdmin && (
        <>
          <Link
            to="/admin/stock"
            onClick={closeMenu}
            className="text-sm font-medium text-slate-600 hover:text-slate-900"
          >
            Manage Stock
          </Link>
          <Link
            to="/products/new"
            onClick={closeMenu}
            className="rounded-md bg-indigo-600 px-4 py-2 text-center text-sm font-medium text-white hover:bg-indigo-700"
          >
            New Product
          </Link>
        </>
      )}
      {user && (
        <Link to="/orders" onClick={closeMenu} className="text-sm font-medium text-slate-600 hover:text-slate-900">
          My Orders
        </Link>
      )}
      <Link to="/cart" onClick={closeMenu} className="text-sm font-medium text-slate-600 hover:text-slate-900">
        Cart{totalItems > 0 ? ` (${totalItems})` : ''}
      </Link>
      {user ? (
        <div className="flex items-center gap-3">
          <span className="text-sm text-slate-600">
            {user.name ?? user.email} <span className="text-slate-400">({user.role})</span>
          </span>
          <button
            type="button"
            onClick={() => {
              logout()
              closeMenu()
            }}
            className="text-sm font-medium text-slate-600 hover:text-slate-900"
          >
            Sign out
          </button>
        </div>
      ) : (
        <Link to="/login" onClick={closeMenu} className="text-sm font-medium text-indigo-600 hover:text-indigo-700">
          Sign in
        </Link>
      )}
    </>
  )

  return (
    <div className="min-h-screen bg-slate-50">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-6 py-4">
          <Link to="/" onClick={closeMenu} className="text-xl font-bold text-indigo-600">
            Cartify
          </Link>

          <nav className="hidden items-center gap-4 sm:flex">{navLinks}</nav>

          <button
            type="button"
            onClick={() => setMenuOpen((open) => !open)}
            aria-label="Toggle menu"
            aria-expanded={menuOpen}
            className="flex items-center justify-center rounded-md p-2 text-slate-600 hover:bg-slate-100 sm:hidden"
          >
            {menuOpen ? (
              <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            ) : (
              <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            )}
          </button>
        </div>

        {menuOpen && (
          <nav className="flex flex-col gap-3 border-t border-slate-200 px-6 py-4 sm:hidden">{navLinks}</nav>
        )}
      </header>
      <main className="mx-auto max-w-5xl px-6 py-8">
        <Outlet />
      </main>
    </div>
  )
}
