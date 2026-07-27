import { apiGet } from './client.js';

export const search = (query, options) => apiGet(`/search?q=${encodeURIComponent(query)}`, options);
