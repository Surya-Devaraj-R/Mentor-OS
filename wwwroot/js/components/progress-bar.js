export function createProgressBar(summary) {
  const container = document.createElement('div');
  container.className = 'rounded-xl border border-white/5 bg-slate-800 p-6 shadow-lg shadow-black/20';

  container.innerHTML = `
    <div class="mb-3 flex items-center justify-between">
      <span class="text-sm font-medium text-slate-400">Overall progress</span>
      <span class="text-sm text-slate-400">${summary.completed} / ${summary.total} completed</span>
    </div>
    <div
      class="h-2 w-full overflow-hidden rounded-full bg-slate-900"
      role="progressbar"
      aria-label="Resources completed"
      aria-valuemin="0"
      aria-valuemax="100"
      aria-valuenow="${summary.percentComplete}"
    >
      <div
        class="h-full rounded-full bg-emerald-500 transition-all duration-500 ease-out"
        style="width: ${summary.percentComplete}%"
      ></div>
    </div>
    <div class="mt-2 text-right text-xs font-medium text-emerald-400">${summary.percentComplete}%</div>
  `;

  return container;
}
