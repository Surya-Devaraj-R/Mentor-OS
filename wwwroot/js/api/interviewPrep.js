import { apiGet, apiPatch } from './client.js';

export const getInterviewQuestions = (type, options) =>
  apiGet(`/interview-prep/questions${type ? `?type=${type}` : ''}`, options);

export const setInterviewQuestionCompleted = (id, completed) =>
  apiPatch(`/interview-prep/questions/${id}/complete`, { completed });
