import { ApiError } from '../api/client.js';

export function createLoadingMessage(text) {
  const el = document.createElement('p');
  el.className = 'text-sm text-slate-400';
  el.textContent = text;
  return el;
}

export function createErrorMessage(error, fallbackText) {
  const el = document.createElement('p');
  el.className = 'text-sm text-red-400';
  el.textContent = error instanceof ApiError ? error.message : fallbackText;
  return el;
}
