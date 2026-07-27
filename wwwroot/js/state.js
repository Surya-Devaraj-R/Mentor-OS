// Tiny central store + pub/sub. This is a CACHE of server truth, not the
// source of truth — mutations always go "API call succeeds -> setState(...)",
// never directly from user input.
const state = {
  progress: null,
};

const listeners = new Map();

export function getState(key) {
  return state[key];
}

export function setState(key, value) {
  state[key] = value;
  listeners.get(key)?.forEach((listener) => listener(value));
}

export function subscribe(key, listener) {
  if (!listeners.has(key)) listeners.set(key, new Set());
  listeners.get(key).add(listener);
  return () => listeners.get(key).delete(listener);
}
