import { useState } from 'react'
import { GoogleLogin, type CredentialResponse } from '@react-oauth/google'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../hooks/useAuth'
import { ErrorAlert } from '../common/ErrorAlert'

export function LoginPage() {
  const { loginWithGoogle, loginAsAdmin, sessionExpired } = useAuth()
  const navigate = useNavigate()

  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [error, setError] = useState<unknown>(null)

  async function handleGoogleSuccess(credential: CredentialResponse) {
    if (!credential.credential) return
    setError(null)
    try {
      await loginWithGoogle(credential.credential)
      navigate('/')
    } catch (err) {
      setError(err)
    }
  }

  async function handleAdminSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setIsSubmitting(true)
    try {
      await loginAsAdmin({ username, password })
      navigate('/')
    } catch (err) {
      setError(err)
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="mx-auto max-w-sm">
      <h1 className="mb-6 text-2xl font-bold text-slate-900">Sign in</h1>

      {sessionExpired && (
        <div className="mb-4 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-800">
          Your session has expired. Please sign in again.
        </div>
      )}

      <div className="rounded-lg border border-slate-200 bg-white p-6">
        <p className="mb-4 text-sm font-medium text-slate-700">Sign in as a customer</p>
        <GoogleLogin
          onSuccess={handleGoogleSuccess}
          onError={() => setError(new Error('Google sign-in failed.'))}
        />

        <div className="my-6 flex items-center gap-3 text-xs text-slate-400">
          <div className="h-px flex-1 bg-slate-200" />
          ADMIN
          <div className="h-px flex-1 bg-slate-200" />
        </div>

        <form onSubmit={handleAdminSubmit} className="space-y-3">
          <div>
            <label className="block text-sm font-medium text-slate-700">Username</label>
            <input
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700">Password</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none"
            />
          </div>
          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full rounded-md bg-slate-800 px-4 py-2 text-sm font-medium text-white hover:bg-slate-900 disabled:opacity-50"
          >
            {isSubmitting ? 'Signing in…' : 'Sign in as Admin'}
          </button>
        </form>

        <div className="mt-4">
          <ErrorAlert error={error} />
        </div>
      </div>
    </div>
  )
}
