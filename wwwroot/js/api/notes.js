import { apiGet, apiPost, apiPut, apiDelete } from './client.js';

export const getNotes = (params = {}, options) => {
  const query = new URLSearchParams();
  if (params.lessonId) query.set('lessonId', params.lessonId);
  if (params.search) query.set('search', params.search);
  const queryString = query.toString();
  return apiGet(`/notes${queryString ? `?${queryString}` : ''}`, options);
};

export const createNote = (note) => apiPost('/notes', note);
export const updateNote = (id, note) => apiPut(`/notes/${id}`, note);
export const deleteNote = (id) => apiDelete(`/notes/${id}`);
