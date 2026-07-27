// Verbatim <pre>, monospace, for anything non-linear (trees, graphs, join
// diagrams) where a flexbox chain doesn't fit. Zero parsing needed.
export function renderAsciiArt(body) {
  const pre = document.createElement('pre');
  pre.className = 'overflow-x-auto rounded-lg border border-white/5 bg-slate-900 p-4 text-xs leading-relaxed text-slate-300';
  pre.textContent = body;
  return pre;
}
