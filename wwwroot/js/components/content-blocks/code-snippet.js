// Plain <pre><code>, textContent only (preserves whitespace, no injection
// risk). Deliberately no manual keyword-coloring: a regex tokenizer that
// rewrites code into colored spans breaks on keywords inside strings/comments
// and isn't worth the maintenance burden for a personal app.
export function renderCodeSnippet({ body, language }) {
  const container = document.createElement('div');
  container.className = 'overflow-hidden rounded-lg border border-white/5 bg-slate-900';

  if (language) {
    const languageBar = document.createElement('div');
    languageBar.className =
      'border-b border-white/5 px-4 py-1.5 text-xs font-medium uppercase tracking-wide text-slate-500';
    languageBar.textContent = language;
    container.appendChild(languageBar);
  }

  const pre = document.createElement('pre');
  pre.className = 'overflow-x-auto p-4 text-xs leading-relaxed text-slate-200';

  const code = document.createElement('code');
  code.textContent = body;
  pre.appendChild(code);
  container.appendChild(pre);

  return container;
}
