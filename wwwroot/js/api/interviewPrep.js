import { apiGet, apiPatch } from './client.js';

export function getInterviewQuestions(params = {}, options) {
  const query = new URLSearchParams();
  if (params.type) query.set('type', params.type);
  if (params.company) query.set('company', params.company);
  const queryString = query.toString();
  return apiGet(`/interview-prep/questions${queryString ? `?${queryString}` : ''}`, options);
}

export const setInterviewQuestionCompleted = (id, completed) =>
  apiPatch(`/interview-prep/questions/${id}/complete`, { completed });

export const getCompanies = (options) => apiGet('/interview-prep/companies', options);
