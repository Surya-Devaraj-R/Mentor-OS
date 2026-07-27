import { apiGet, apiPost, apiPatch, apiDelete } from './client.js';

export const getPlanItemsForDate = (date, options) => apiGet(`/planner/${date}`, options);
export const createPlanItem = (item) => apiPost('/planner', item);
export const setPlanItemDone = (id, done) => apiPatch(`/planner/${id}/done`, { done });
export const deletePlanItem = (id) => apiDelete(`/planner/${id}`);
