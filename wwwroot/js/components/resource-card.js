import { ICONS } from './icons.js';

// Pure createX(props) -> HTMLElement: no API calls, no store access, no
// listeners attached internally. The owning view wires one delegated
// listener on its container and reads `data-resource-id` off the checkbox.
export function createResourceCard(resource) {
  const card = document.createElement('article');
  card.dataset.resourceId = String(resource.id);
  card.className = getCardClassName(resource.isCompleted);

  card.innerHTML = `
    <div class="flex items-start justify-between gap-3">
      <span class="flex h-10 w-10 items-center justify-center rounded-lg bg-emerald-500/10 text-emerald-400">
        ${ICONS[resource.iconKey] ?? ''}
      </span>
      <label class="flex cursor-pointer select-none items-center gap-2 text-xs text-slate-400">
        Completed
        <input
          type="checkbox"
          data-resource-id="${resource.id}"
          aria-label="Mark ${resource.title} as completed"
          class="h-4 w-4 rounded accent-emerald-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50"
          ${resource.isCompleted ? 'checked' : ''}
        />
      </label>
    </div>
    <div class="flex flex-col gap-1">
      <h3 class="text-base font-semibold text-slate-50">${resource.title}</h3>
      <p class="text-sm text-slate-400">${resource.label}</p>
    </div>
    <a
      href="${resource.url}"
      target="_blank"
      rel="noopener noreferrer"
      class="mt-auto inline-flex items-center justify-center rounded-lg border border-white/10 py-2 text-sm font-medium text-slate-50 transition duration-300 hover:border-emerald-500/40 hover:bg-emerald-500/10 hover:text-emerald-400 focus:outline-none focus:ring-2 focus:ring-emerald-500/50"
    >
      Visit resource
      <span class="sr-only"> (opens ${resource.title} in a new tab)</span>
    </a>
  `;

  return card;
}

export function getCardClassName(isCompleted) {
  const borderClass = isCompleted ? 'border-emerald-500/30' : 'border-white/5';
  return `flex flex-col gap-4 rounded-xl border ${borderClass} bg-slate-800 p-6 shadow-lg shadow-black/20 transition duration-300 hover:-translate-y-1 hover:border-emerald-500/30 hover:shadow-emerald-500/5`;
}
