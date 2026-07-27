import { apiGet } from './client.js';

export const getModulesForTopic = (topicSlug, options) => apiGet(`/topics/${topicSlug}/modules`, options);
export const getModule = (slug, options) => apiGet(`/modules/${slug}`, options);
