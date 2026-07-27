import { apiGet } from './client.js';

export const getCurrentStreak = (options) => apiGet('/streaks/current', options);
