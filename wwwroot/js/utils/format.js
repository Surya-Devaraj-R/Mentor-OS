export function toDateKey(date) {
  return date.toISOString().slice(0, 10);
}

export function addDays(dateKey, delta) {
  const date = new Date(`${dateKey}T00:00:00`);
  date.setDate(date.getDate() + delta);
  return toDateKey(date);
}

export function formatDateLabel(dateKey) {
  const date = new Date(`${dateKey}T00:00:00`);
  return date.toLocaleDateString(undefined, { weekday: 'long', month: 'long', day: 'numeric' });
}
