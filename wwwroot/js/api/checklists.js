import { apiPatch } from './client.js';

export const setChecklistItemCompleted = (id, completed) => apiPatch(`/checklist-items/${id}/complete`, { completed });
