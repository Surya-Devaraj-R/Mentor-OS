// Hash-based routing: every URL of the form /#/anything is one HTTP request
// (GET /) that the static-file host already serves. A hard refresh on any
// route never 404s and never needs a Program.cs change.
const routes = [];

export function registerRoute(pattern, handler) {
  if (pattern === '*') {
    routes.push({ regex: /^.*$/, paramNames: [], handler });
    return;
  }

  const paramNames = [];
  const regexBody = pattern
    .split('/')
    .map((segment) => {
      if (segment.startsWith(':')) {
        paramNames.push(segment.slice(1));
        return '([^/]+)';
      }
      return segment.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    })
    .join('/');

  routes.push({ regex: new RegExp(`^${regexBody}$`), paramNames, handler });
}

function parseHash() {
  const raw = location.hash.slice(1) || '/';
  const [path, queryString = ''] = raw.split('?');
  return { path: path || '/', query: new URLSearchParams(queryString) };
}

let activeTeardown = null;

async function renderRoute() {
  const { path, query } = parseHash();
  const match = routes.find((route) => route.regex.test(path));
  const root = document.getElementById('app-root');
  const announcer = document.getElementById('route-announcer');

  if (activeTeardown) {
    activeTeardown();
    activeTeardown = null;
  }

  if (!match) return;

  const values = match.regex.exec(path).slice(1);
  const params = Object.fromEntries(
    match.paramNames.map((name, index) => [name, decodeURIComponent(values[index])]),
  );

  root.setAttribute('aria-busy', 'true');
  const teardown = await match.handler(params, query, root);
  root.removeAttribute('aria-busy');

  if (typeof teardown === 'function') activeTeardown = teardown;
  // Hash routing never triggers a page reload, so screen readers won't
  // announce the new view unless we do it ourselves via this live region.
  announcer.textContent = document.title;
}

export function navigate(hash) {
  location.hash = hash;
}

// Re-runs the current route's handler without changing the URL — for when a
// view mutates data behind its own route (e.g. adding a planner item) and
// setting location.hash to its current value wouldn't fire 'hashchange'.
export function refresh() {
  return renderRoute();
}

export function initRouter() {
  window.addEventListener('hashchange', renderRoute);
  renderRoute();
}
