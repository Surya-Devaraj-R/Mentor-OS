import { search } from '../api/search.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';

const GROUP_LABELS = {
  Topic: 'Topics',
  Module: 'Modules',
  Lesson: 'Lessons',
  Exercise: 'Exercises',
  InterviewQuestion: 'Interview Prep',
  Note: 'Notes',
  Resource: 'Resources',
};

// Route: /search?q=...
export async function renderSearchView(params, query, root) {
  const term = query.get('q') || '';
  document.title = term ? `"${term}" · Search · Mentor OS` : 'Search · Mentor OS';
  root.replaceChildren(createLoadingMessage('Searching…'));

  if (!term.trim()) {
    root.replaceChildren(buildEmptyState());
    return;
  }

  const controller = new AbortController();

  try {
    const response = await search(term, { signal: controller.signal });
    root.replaceChildren(buildResultsView(response));
  } catch (error) {
    if (error.name === 'AbortError') return;
    root.replaceChildren(createErrorMessage(error, 'Something went wrong while searching.'));
  }

  return () => controller.abort();
}

function buildEmptyState() {
  const message = document.createElement('p');
  message.className = 'text-sm text-slate-400';
  message.textContent = 'Type a search term in the box above to get started.';
  return message;
}

function buildResultsView(response) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-6';

  const heading = document.createElement('h1');
  heading.className = 'text-2xl font-semibold tracking-tight text-slate-50';
  heading.textContent = `Results for "${response.query}"`;
  container.appendChild(heading);

  if (response.groups.length === 0) {
    const empty = document.createElement('p');
    empty.className = 'text-sm text-slate-400';
    empty.textContent = 'No matches found.';
    container.appendChild(empty);
    return container;
  }

  response.groups.forEach((group) => container.appendChild(createGroupSection(group)));
  return container;
}

function createGroupSection(group) {
  const section = document.createElement('section');
  section.className = 'flex flex-col gap-3';

  const heading = document.createElement('h2');
  heading.className = 'text-lg font-semibold tracking-tight text-slate-50';
  heading.textContent = GROUP_LABELS[group.entityType] ?? group.entityType;
  section.appendChild(heading);

  const list = document.createElement('div');
  list.className = 'flex flex-col gap-2';
  group.items.forEach((item) => list.appendChild(createResultRow(item)));

  section.appendChild(list);
  return section;
}

function createResultRow(item) {
  const link = document.createElement('a');
  link.href = item.navigateHash;
  link.className =
    'flex flex-col gap-1 rounded-xl border border-white/5 bg-slate-800 p-4 shadow-lg shadow-black/20 transition duration-300 hover:-translate-y-1 hover:border-emerald-500/30 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';

  const title = document.createElement('h3');
  title.className = 'text-sm font-semibold text-slate-50';
  title.textContent = item.title;

  const snippet = document.createElement('p');
  snippet.className = 'truncate text-xs text-slate-400';
  snippet.textContent = item.snippet;

  link.append(title, snippet);
  return link;
}
