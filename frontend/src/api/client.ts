import { ApiError, type ProblemDetails } from './types'

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5283/api'

let authToken: string | null = null

export function setAuthToken(token: string | null) {
  authToken = token
}

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${BASE_URL}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(authToken ? { Authorization: `Bearer ${authToken}` } : {}),
      ...options?.headers,
    },
  })

  if (!response.ok) {
    let detail = response.statusText
    let title: string | undefined

    try {
      const problem: ProblemDetails = await response.json()
      detail = problem.detail ?? detail
      title = problem.title
    } catch {
      // response body wasn't JSON — fall back to statusText already set above
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
