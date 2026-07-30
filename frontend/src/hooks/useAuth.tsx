import { createContext, useContext, useEffect, useState, type ReactNode } from 'react'
import { authApi } from '../api/auth'
import { setAuthToken, setUnauthorizedHandler } from '../api/client'
import type { AdminLoginRequest, AuthResponse, UserRole } from '../api/types'

interface AuthUser {
  email: string
  name: string | null
  role: UserRole
}

interface AuthContextValue {
  user: AuthUser | null
  isAdmin: boolean
  sessionExpired: boolean
  loginWithGoogle: (idToken: string) => Promise<void>
  loginAsAdmin: (request: AdminLoginRequest) => Promise<void>
  logout: () => void
}

const STORAGE_KEY = 'cartify.auth'

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

function persist(response: AuthResponse | null) {
  if (response) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(response))
  } else {
    localStorage.removeItem(STORAGE_KEY)
  }
}

function readPersisted(): AuthResponse | null {
  const raw = localStorage.getItem(STORAGE_KEY)
  if (!raw) return null
  try {
    return JSON.parse(raw) as AuthResponse
  } catch {
    return null
  }
}

// Reads localStorage synchronously as the initial state (not in a useEffect) so a logged-in
// user's identity is available on the very first render — otherwise route guards like
// AdminRoute/RequireAuthRoute see `user === null` on that first render and redirect to
// /login before the effect has a chance to run, even though a valid session exists.
function initialUser(): AuthUser | null {
  const stored = readPersisted()
  if (!stored) return null
  setAuthToken(stored.token)
  return { email: stored.email, name: stored.name, role: stored.role }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(initialUser)
  const [sessionExpired, setSessionExpired] = useState(false)

  function applyAuthResponse(response: AuthResponse) {
    setAuthToken(response.token)
    setUser({ email: response.email, name: response.name, role: response.role })
    setSessionExpired(false)
    persist(response)
  }

  async function loginWithGoogle(idToken: string) {
    const response = await authApi.loginWithGoogle({ idToken })
    applyAuthResponse(response)
  }

  async function loginAsAdmin(request: AdminLoginRequest) {
    const response = await authApi.loginAsAdmin(request)
    applyAuthResponse(response)
  }

  function logout() {
    setAuthToken(null)
    setUser(null)
    persist(null)
  }

  // If a token we're holding gets rejected as expired/invalid by the backend, clear the
  // session automatically (rather than leaving the UI stuck showing "logged in" while every
  // authenticated call silently fails) and flag it so the login page can explain why the
  // user landed there instead of just silently bouncing them.
  function handleSessionExpired() {
    logout()
    setSessionExpired(true)
  }

  useEffect(() => {
    setUnauthorizedHandler(handleSessionExpired)
    return () => setUnauthorizedHandler(null)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const value: AuthContextValue = {
    user,
    isAdmin: user?.role === 'Admin',
    sessionExpired,
    loginWithGoogle,
    loginAsAdmin,
    logout,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}
