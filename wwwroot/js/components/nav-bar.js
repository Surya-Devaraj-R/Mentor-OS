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
  nav.className = 'sticky top-0 z-10 border-b border-white/5 bg-slate-900/80 shadow-lg shadow-black/10 backdrop-blur-md';
  nav.setAttribute('aria-label', 'Primary');

  const inner = document.createElement('div');
  inner.className = 'mx-auto flex w-full max-w-6xl items-center justify-between px-6 py-3.5';

  const brand = document.createElement('a');
  brand.href = '#/';
  brand.className =
    'flex items-center gap-2 rounded text-sm font-bold tracking-tight text-slate-50 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  brand.innerHTML = `
    <span class="flex h-7 w-7 items-center justify-center rounded-lg bg-gradient-to-br from-emerald-400 to-violet-500 text-xs font-extrabold text-slate-900 shadow shadow-emerald-500/30">M</span>
    <span>Mentor OS</span>
  `;

  // Full inline nav — only shown from lg up. Below that, everything lives
  // behind the hamburger toggle in the mobile panel instead.
  const desktopLinks = document.createElement('div');
  desktopLinks.className = 'hidden items-center gap-1 lg:flex';
  NAV_ITEMS.forEach((item) => desktopLinks.appendChild(createNavLink(item)));

  const streakBadge = document.createElement('span');
  streakBadge.className =
    'hidden items-center gap-1 rounded-full border border-amber-500/20 bg-amber-500/10 px-2.5 py-1 text-xs font-semibold text-amber-400';
  streakBadge.setAttribute('aria-live', 'polite');

  const menuButton = document.createElement('button');
  menuButton.type = 'button';
  menuButton.className =
    'flex h-9 w-9 items-center justify-center rounded-lg text-lg text-slate-300 transition duration-300 hover:bg-white/5 hover:text-slate-50 focus:outline-none focus:ring-2 focus:ring-emerald-500/50 lg:hidden';
  menuButton.setAttribute('aria-label', 'Toggle navigation menu');
  menuButton.setAttribute('aria-expanded', 'false');
  menuButton.setAttribute('aria-controls', 'mobile-nav-panel');
  menuButton.textContent = '☰';

  const right = document.createElement('div');
  right.className = 'flex items-center gap-4';
  right.append(desktopLinks, createSearchForm('hidden lg:block'), streakBadge, menuButton);

  inner.append(brand, right);

  const mobilePanel = document.createElement('div');
  mobilePanel.id = 'mobile-nav-panel';
  mobilePanel.className = 'hidden flex-col gap-3 border-t border-white/5 px-6 py-4 lg:hidden';

  const mobileLinks = document.createElement('div');
  mobileLinks.className = 'flex flex-col gap-1';
  NAV_ITEMS.forEach((item) => mobileLinks.appendChild(createNavLink(item)));

  mobilePanel.append(createSearchForm('block'), mobileLinks);

  const closeMobileMenu = () => {
    mobilePanel.classList.add('hidden');
    mobilePanel.classList.remove('flex');
    menuButton.setAttribute('aria-expanded', 'false');
    menuButton.textContent = '☰';
  };

  menuButton.addEventListener('click', () => {
    const isOpen = !mobilePanel.classList.contains('hidden');
    if (isOpen) {
      closeMobileMenu();
    } else {
      mobilePanel.classList.remove('hidden');
      mobilePanel.classList.add('flex');
      menuButton.setAttribute('aria-expanded', 'true');
      menuButton.textContent = '✕';
    }
  });

  mobileLinks.addEventListener('click', (event) => {
    if (event.target.closest('a[data-nav-hash]')) closeMobileMenu();
  });

  nav.append(inner, mobilePanel);
  mountEl.replaceChildren(nav);

  window.addEventListener('hashchange', () => {
    updateActiveLink(nav);
    closeMobileMenu();
  });
  updateActiveLink(nav);

  const refreshStreakBadge = () => loadStreakBadge(streakBadge);
  refreshStreakBadge();
  // Progress changes (completing a resource/lesson/planner item) are the
  // only paths that can move the streak today, so re-fetch on each one.
  subscribe('progress', refreshStreakBadge);
}

async function loadStreakBadge(badgeEl) {
  try {
    const streak = await getCurrentStreak();
    const hasStreak = streak.currentStreakDays > 0;
    badgeEl.textContent = hasStreak ? `🔥 ${streak.currentStreakDays}` : '';
    badgeEl.classList.toggle('hidden', !hasStreak);
    badgeEl.classList.toggle('flex', hasStreak);
  } catch {
    badgeEl.textContent = '';
    badgeEl.classList.add('hidden');
    badgeEl.classList.remove('flex');
  }
}

function createSearchForm(visibilityClass) {
  const form = document.createElement('form');
  form.className = visibilityClass;
  form.setAttribute('role', 'search');

  const input = document.createElement('input');
  input.type = 'search';
  input.placeholder = 'Search…';
  input.className =
    'w-full rounded-lg border border-white/10 bg-slate-800 px-3 py-1.5 text-xs text-slate-50 placeholder:text-slate-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50 lg:w-40';
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

function updateActiveLink(nav) {
  const currentHash = location.hash || '#/';
  nav.querySelectorAll('a[data-nav-hash]').forEach((link) => {
    const isActive = link.dataset.navHash === currentHash;
    link.classList.toggle('bg-emerald-500/10', isActive);
    link.classList.toggle('text-emerald-400', isActive);
    link.classList.toggle('text-slate-400', !isActive);
  });
}
