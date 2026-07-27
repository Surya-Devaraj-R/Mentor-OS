// items: [{ label, hash? }] — the last item should omit `hash` (current page).
export function createBreadcrumb(items) {
  const nav = document.createElement('nav');
  nav.setAttribute('aria-label', 'Breadcrumb');
  nav.className = 'flex flex-wrap items-center gap-1.5 text-xs text-slate-400';

  items.forEach((item, index) => {
    if (index > 0) {
      const separator = document.createElement('span');
      separator.setAttribute('aria-hidden', 'true');
      separator.textContent = '/';
      nav.appendChild(separator);
    }

    if (item.hash) {
      const link = document.createElement('a');
      link.href = item.hash;
      link.className = 'rounded hover:text-slate-50 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
      link.textContent = item.label;
      nav.appendChild(link);
    } else {
      const current = document.createElement('span');
      current.className = 'text-slate-200';
      current.setAttribute('aria-current', 'page');
      current.textContent = item.label;
      nav.appendChild(current);
    }
  });

  return nav;
}
