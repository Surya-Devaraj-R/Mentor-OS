import { getCurrentStreak } from '../api/streaks.js';
import { subscribe } from '../state.js';
import { navigate } from '../router.js';

const NAV_ITEMS = [
  { hash: '#/', label: 'Dashboard' },
  { hash: '#/planner', label: 'Planner' },
  { hash: '#/practice', label: 'Practice' },
  { hash: '#/interview-prep', label: 'Interview Prep' },
  { hash: '#/notes', label: 'Notes' },
  { hash: '#/resources', label: 'Resources' },
];

export function renderNavBar(mountEl) {
  const nav = document.createElement('nav');
  nav.className = 'sticky top-0 z-10 border-b border-white/5 bg-slate-900/95 backdrop-blur';
  nav.setAttribute('aria-label', 'Primary');

  const inner = document.createElement('div');
  inner.className = 'mx-auto flex w-full max-w-6xl items-center justify-between px-6 py-4';

  const brand = document.createElement('a');
  brand.href = '#/';
  brand.className =
    'rounded text-sm font-semibold tracking-tight text-slate-50 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  brand.textContent = 'Mentor OS';

  const links = document.createElement('div');
  links.className = 'flex items-center gap-1';
  NAV_ITEMS.forEach((item) => links.appendChild(createNavLink(item)));

  const streakBadge = document.createElement('span');
  streakBadge.className = 'text-xs font-medium text-emerald-400';
  streakBadge.setAttribute('aria-live', 'polite');

  const right = document.createElement('div');
  right.className = 'flex items-center gap-4';
  right.append(links, createSearchForm(), streakBadge);

  inner.append(brand, right);
  nav.appendChild(inner);
  mountEl.replaceChildren(nav);

  window.addEventListener('hashchange', () => updateActiveLink(links));
  updateActiveLink(links);

  const refreshStreakBadge = () => loadStreakBadge(streakBadge);
  refreshStreakBadge();
  // Progress changes (completing a resource/lesson/planner item) are the
  // only paths that can move the streak today, so re-fetch on each one.
  subscribe('progress', refreshStreakBadge);
}

async function loadStreakBadge(badgeEl) {
  try {
    const streak = await getCurrentStreak();
    badgeEl.textContent = streak.currentStreakDays > 0 ? `🔥 ${streak.currentStreakDays}` : '';
  } catch {
    badgeEl.textContent = '';
  }
}

function createSearchForm() {
  const form = document.createElement('form');
  form.className = 'hidden sm:block';
  form.setAttribute('role', 'search');

  const input = document.createElement('input');
  input.type = 'search';
  input.placeholder = 'Search…';
  input.className =
    'w-40 rounded-lg border border-white/10 bg-slate-800 px-3 py-1.5 text-xs text-slate-50 placeholder:text-slate-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  input.setAttribute('aria-label', 'Search all content');

  form.appendChild(input);
  form.addEventListener('submit', (event) => {
    event.preventDefault();
    const term = input.value.trim();
    if (term) navigate(`/search?q=${encodeURIComponent(term)}`);
  });

  return form;
}

function createNavLink(item) {
  const link = document.createElement('a');
  link.href = item.hash;
  link.dataset.navHash = item.hash;
  link.className =
    'rounded-lg px-3 py-2 text-sm font-medium text-slate-400 transition duration-300 hover:text-slate-50 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  link.textContent = item.label;
  return link;
}

function updateActiveLink(links) {
  const currentHash = location.hash || '#/';
  links.querySelectorAll('a[data-nav-hash]').forEach((link) => {
    const isActive = link.dataset.navHash === currentHash;
    link.classList.toggle('text-emerald-400', isActive);
    link.classList.toggle('text-slate-400', !isActive);
  });
}
