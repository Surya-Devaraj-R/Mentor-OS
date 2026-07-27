import { apiGet, apiPost } from './client.js';

export function getExercises(params = {}, options) {
  const query = new URLSearchParams();
  if (params.lessonId) query.set('lessonId', params.lessonId);
  if (params.difficulty) query.set('difficulty', params.difficulty);
  if (params.interviewOnly) query.set('interviewOnly', 'true');
  const queryString = query.toString();
  return apiGet(`/exercises${queryString ? `?${queryString}` : ''}`, options);
}

export const getExercise = (slug, options) => apiGet(`/exercises/${slug}`, options);
export const submitExerciseAttempt = (id, submission) => apiPost(`/exercises/${id}/submissions`, submission);
