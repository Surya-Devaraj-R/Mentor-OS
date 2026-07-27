export async function renderNotFoundView(params, query, root) {
  document.title = 'Not Found · Mentor OS';

  const container = document.createElement('div');
  container.className = 'flex flex-1 flex-col items-center justify-center gap-2 py-20 text-center';
  container.innerHTML = `
    <h1 class="text-xl font-semibold text-slate-50">Page not found</h1>
    <p class="text-sm text-slate-400">That page doesn't exist yet.</p>
    <a href="#/" class="mt-4 text-sm font-medium text-emerald-400 hover:underline">Back to dashboard</a>
  `;

  root.replaceChildren(container);
}
