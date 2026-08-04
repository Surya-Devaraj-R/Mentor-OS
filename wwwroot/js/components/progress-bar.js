export function createProgressBar(summary) {
  const container = document.createElement('div');
  container.className =
    'rounded-2xl border border-white/5 bg-slate-800/60 p-6 shadow-lg shadow-black/20 ring-1 ring-white/5 backdrop-blur';

  container.innerHTML = `
    <div class="mb-3 flex items-center justify-between">
      <span class="text-sm font-semibold text-slate-300">Overall progress</span>
      <span class="text-sm text-slate-400">${summary.completed} <span class="text-slate-600">/</span> ${summary.total} completed</span>
    </div>
    <div
      class="h-2.5 w-full overflow-hidden rounded-full bg-slate-950/80 ring-1 ring-white/5"
      role="progressbar"
      aria-label="Resources completed"
      aria-valuemin="0"
      aria-valuemax="100"
      aria-valuenow="${summary.percentComplete}"
    >
      <div
        class="h-full rounded-full bg-gradient-to-r from-emerald-500 to-violet-500 shadow-[0_0_10px_rgba(52,211,153,0.5),0_0_16px_rgba(167,139,250,0.35)] transition-all duration-500 ease-out"
        style="width: ${summary.percentComplete}%"
      ></div>
    </div>
    <div class="mt-2 text-right text-xs font-semibold text-emerald-400">${summary.percentComplete}%</div>
  `;

  return container;
}
