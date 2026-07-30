import { ApiError, type ProblemDetails } from './types'

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5283/api'

let authToken: string | null = null

export function setAuthToken(token: string | null) {
  authToken = token
}

// Called when a request made WITH a token comes back 401 — the token is expired or
// otherwise invalid server-side, so the app's idea of "logged in" is stale. Registered
// by AuthProvider so it can clear the session; kept here (not imported directly) since
// this module has no dependency on React/auth state otherwise.
let unauthorizedHandler: (() => void) | null = null

export function setUnauthorizedHandler(handler: (() => void) | null) {
  unauthorizedHandler = handler
}

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const hadToken = !!authToken

  const response = await fetch(`${BASE_URL}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(authToken ? { Authorization: `Bearer ${authToken}` } : {}),
      ...options?.headers,
    },
  })

  if (!response.ok) {
    // response.statusText is spec'd to always be "" over HTTP/2 (no reason phrase in the
    // protocol), which both Vercel and Azure Container Apps serve over — so it can't be
    // relied on as a fallback message.
    let detail = `Request failed (${response.status}).`
    let title: string | undefined

    try {
      const problem: ProblemDetails = await response.json()
      detail = problem.detail ?? detail
      title = problem.title
    } catch {
      // response body wasn't JSON — keep the status-based fallback above
    }

    if (response.status === 401) {
      detail = 'Your session has expired. Please sign in again.'
      if (hadToken) {
        unauthorizedHandler?.()
      }
    }

    throw new ApiError(response.status, detail, title)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json() as Promise<T>
}

export const apiClient = {
  get: <T>(path: string) => request<T>(path),
  post: <T>(path: string, body: unknown) =>
    request<T>(path, { method: 'POST', body: JSON.stringify(body) }),
  put: <T>(path: string, body: unknown) =>
    request<T>(path, { method: 'PUT', body: JSON.stringify(body) }),
  delete: <T>(path: string) => request<T>(path, { method: 'DELETE' }),
}
