// Hand-rolled subset of markdown: **bold**, `inline code`, "- " bullets,
// "1. " numbered lists, blank-line-separated paragraphs. Nothing else.
// Always builds real DOM nodes via textContent — never innerHTML on
// interpolated content, even though this app is currently single-user.
const INLINE_PATTERN = /(\*\*[^*]+\*\*|`[^`]+`)/g;

function appendInline(parent, text) {
  for (const part of text.split(INLINE_PATTERN)) {
    if (!part) continue;

    if (part.startsWith('**') && part.endsWith('**')) {
      const strong = document.createElement('strong');
      strong.className = 'font-semibold text-slate-50';
      strong.textContent = part.slice(2, -2);
      parent.appendChild(strong);
    } else if (part.startsWith('`') && part.endsWith('`')) {
      const code = document.createElement('code');
      code.className = 'rounded bg-slate-900 px-1.5 py-0.5 font-mono text-xs text-emerald-300';
      code.textContent = part.slice(1, -1);
      parent.appendChild(code);
    } else {
      parent.appendChild(document.createTextNode(part));
    }
  }
}

const isBulletLine = (line) => /^-\s+/.test(line);
const isNumberedLine = (line) => /^\d+\.\s+/.test(line);

function renderBlock(text) {
  const lines = text.split('\n').map((line) => line.trim()).filter(Boolean);

  if (lines.length > 0 && lines.every(isBulletLine)) {
    const list = document.createElement('ul');
    list.className = 'list-disc space-y-1 pl-5';
    lines.forEach((line) => {
      const item = document.createElement('li');
      appendInline(item, line.replace(/^-\s+/, ''));
      list.appendChild(item);
    });
    return list;
  }

  if (lines.length > 0 && lines.every(isNumberedLine)) {
    const list = document.createElement('ol');
    list.className = 'list-decimal space-y-1 pl-5';
    lines.forEach((line) => {
      const item = document.createElement('li');
      appendInline(item, line.replace(/^\d+\.\s+/, ''));
      list.appendChild(item);
    });
    return list;
  }

  const paragraph = document.createElement('p');
  appendInline(paragraph, lines.join(' '));
  return paragraph;
}

export function renderMiniMarkdown(text) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-3 text-sm leading-relaxed text-slate-300';

  text
    .trim()
    .split(/\n\s*\n/)
    .filter((block) => block.trim())
    .forEach((block) => container.appendChild(renderBlock(block)));

  return container;
}
