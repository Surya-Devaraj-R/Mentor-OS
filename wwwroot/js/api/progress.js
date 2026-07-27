import { apiGet } from './client.js';

export const getProgressSummary = (options) => apiGet('/progress/summary', options);
