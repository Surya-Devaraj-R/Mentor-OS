import { apiGet } from './client.js';

export const getProjectForTopic = (topicSlug, options) => apiGet(`/topics/${topicSlug}/project`, options);
