import { apiGet, apiPatch } from './client.js';

export const getResources = (options) => apiGet('/resources', options);
export const setResourceCompleted = (id, completed) => apiPatch(`/resources/${id}/complete`, { completed });
