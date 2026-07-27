import { apiGet, apiPatch } from './client.js';

export const getLesson = (slug, options) => apiGet(`/lessons/${slug}`, options);
export const setLessonCompleted = (id, completed) => apiPatch(`/lessons/${id}/complete`, { completed });
