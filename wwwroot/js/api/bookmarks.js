import { apiGet, apiPost, apiDelete } from './client.js';

export const getBookmarks = (options) => apiGet('/bookmarks', options);
export const createBookmark = (bookmark) => apiPost('/bookmarks', bookmark);
export const deleteBookmark = (id) => apiDelete(`/bookmarks/${id}`);
