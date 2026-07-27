import { renderMiniMarkdown } from './mini-markdown.js';
import { renderCodeSnippet } from './code-snippet.js';
import { renderStructuredSteps } from './structured-steps.js';
import { renderAsciiArt } from './ascii-art.js';

const BLOCK_LABELS = {
  Notes: 'Notes',
  CheatSheet: 'Cheat Sheet',
  CodeSnippet: 'Code Example',
  Diagram: 'Diagram',
  BestPractice: 'Best Practice',
  InterviewTip: 'Interview Tip',
  CommonMistake: 'Common Mistake',
};

export function renderContentBlock(block) {
  const wrapper = document.createElement('section');
  wrapper.className = 'flex flex-col gap-3 rounded-xl border border-white/5 bg-slate-800 p-5 shadow-lg shadow-black/20';
  wrapper.setAttribute('aria-label', BLOCK_LABELS[block.blockType] ?? block.blockType);

  const heading = document.createElement('h3');
  heading.className = 'text-xs font-semibold uppercase tracking-wide text-emerald-400';
  heading.textContent = block.title || BLOCK_LABELS[block.blockType] || block.blockType;

  wrapper.append(heading, renderBlockBody(block));
  return wrapper;
}

function renderBlockBody(block) {
  // CodeSnippet is dispatched on blockType, not bodyFormat: it's always a
  // raw <pre><code>, regardless of what BodyFormat happens to be stored.
  if (block.blockType === 'CodeSnippet') {
    return renderCodeSnippet({ body: block.body, language: block.language });
  }

  switch (block.bodyFormat) {
    case 'MiniMarkdown':
      return renderMiniMarkdown(block.body);
    case 'StructuredSteps':
      return renderStructuredSteps(block.body);
    case 'AsciiArt':
      return renderAsciiArt(block.body);
    default:
      return renderPlainText(block.body);
  }
}

function renderPlainText(body) {
  const paragraph = document.createElement('p');
  paragraph.className = 'whitespace-pre-line text-sm leading-relaxed text-slate-300';
  paragraph.textContent = body;
  return paragraph;
}
