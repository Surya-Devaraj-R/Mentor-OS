const BASE_URL = '/api';

export class ApiError extends Error {
  constructor(message, status, body) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.body = body;
  }
}

async function request(path, { method = 'GET', body, signal } = {}) {
  const response = await fetch(`${BASE_URL}${path}`, {
    method,
    headers: body ? { 'Content-Type': 'application/json' } : undefined,
    body: body ? JSON.stringify(body) : undefined,
    signal,
  });

  if (response.status === 204) return null;

  const text = await response.text();
  const data = text ? JSON.parse(text) : null;

  if (!response.ok) {
    const message = data?.title ?? data?.message ?? `Request failed (${response.status})`;
    throw new ApiError(message, response.status, data);
  }

  return data;
}

export const apiGet = (path, options) => request(path, { ...options, method: 'GET' });
export const apiPost = (path, body, options) => request(path, { ...options, method: 'POST', body });
export const apiPut = (path, body, options) => request(path, { ...options, method: 'PUT', body });
export const apiPatch = (path, body, options) => request(path, { ...options, method: 'PATCH', body });
export const apiDelete = (path, options) => request(path, { ...options, method: 'DELETE' });
