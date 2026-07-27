import { apiGet } from './client.js';

export const getTopics = (options) => apiGet('/topics', options);
export const getTopic = (slug, options) => apiGet(`/topics/${slug}`, options);
