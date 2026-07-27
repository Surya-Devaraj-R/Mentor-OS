import { getNotes, createNote, updateNote, deleteNote } from '../api/notes.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';
import { renderMiniMarkdown } from '../components/content-blocks/mini-markdown.js';
import { refresh } from '../router.js';

// Route: /notes
export async function renderNotesView(params, query, root) {
  document.title = 'Notes · Mentor OS';
  root.replaceChildren(createLoadingMessage('Loading notes…'));

  const controller = new AbortController();

  try {
    const notes = await getNotes({}, { signal: controller.signal });
    root.replaceChildren(buildView(notes));
  } catch (error) {
    if (error.name === 'AbortError') return;
    root.replaceChildren(createErrorMessage(error, 'Something went wrong loading notes.'));
  }

  return () => controller.abort();
}

function buildView(notes) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-6';

  const heading = document.createElement('h1');
  heading.className = 'text-2xl font-semibold tracking-tight text-slate-50';
  heading.textContent = 'Notes';

  const list = document.createElement('div');
  list.className = 'flex flex-col gap-4';

  if (notes.length === 0) {
    const empty = document.createElement('p');
    empty.className = 'text-sm text-slate-400';
    empty.textContent = 'No notes yet — add your first one below.';
    list.appendChild(empty);
  } else {
    notes.forEach((note) => list.appendChild(createNoteCard(note)));
  }

  container.append(heading, createAddNoteForm(), list);
  return container;
}

function createAddNoteForm() {
  const form = document.createElement('form');
  form.className = 'flex flex-col gap-2 rounded-xl border border-white/5 bg-slate-800 p-5';

  const titleInput = document.createElement('input');
  titleInput.type = 'text';
  titleInput.placeholder = 'Title (optional)';
  titleInput.className =
    'rounded-lg border border-white/10 bg-slate-900 px-3 py-2 text-sm text-slate-50 placeholder:text-slate-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  titleInput.setAttribute('aria-label', 'Note title (optional)');

  const bodyInput = document.createElement('textarea');
  bodyInput.rows = 3;
  bodyInput.required = true;
  bodyInput.placeholder = 'Write a note… supports **bold**, `code`, and - bullet lists.';
  bodyInput.className =
    'rounded-lg border border-white/10 bg-slate-900 px-3 py-2 text-sm text-slate-50 placeholder:text-slate-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  bodyInput.setAttribute('aria-label', 'Note body');

  const submitButton = document.createElement('button');
  submitButton.type = 'submit';
  submitButton.className =
    'self-end rounded-lg border border-emerald-500/40 bg-emerald-500/10 px-4 py-2 text-sm font-medium text-emerald-400 transition duration-300 hover:bg-emerald-500/20 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  submitButton.textContent = 'Add Note';

  form.append(titleInput, bodyInput, submitButton);

  form.addEventListener('submit', async (event) => {
    event.preventDefault();
    const body = bodyInput.value.trim();
    if (!body) return;

    try {
      await createNote({ lessonId: null, title: titleInput.value.trim() || null, body });
      await refresh();
    } catch (error) {
      console.error('Failed to create note:', error);
    }
  });

  return form;
}

function createNoteCard(note) {
  const card = document.createElement('article');
  card.className = 'flex flex-col gap-3 rounded-xl border border-white/5 bg-slate-800 p-5 shadow-lg shadow-black/20';

  const header = document.createElement('div');
  header.className = 'flex items-start justify-between gap-3';

  const titleCol = document.createElement('div');
  if (note.title) {
    const title = document.createElement('h3');
    title.className = 'text-base font-semibold text-slate-50';
    title.textContent = note.title;
    titleCol.appendChild(title);
  }
  if (note.lessonTitle) {
    const source = document.createElement('span');
    source.className = 'text-xs text-slate-500';
    source.textContent = `From: ${note.lessonTitle}`;
    titleCol.appendChild(source);
  }

  const actions = document.createElement('div');
  actions.className = 'flex shrink-0 gap-2 text-xs';

  const editButton = document.createElement('button');
  editButton.type = 'button';
  editButton.className = 'rounded text-slate-400 hover:text-emerald-400 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  editButton.textContent = 'Edit';

  const deleteButton = document.createElement('button');
  deleteButton.type = 'button';
  deleteButton.className = 'rounded text-slate-400 hover:text-red-400 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  deleteButton.textContent = 'Delete';

  actions.append(editButton, deleteButton);
  header.append(titleCol, actions);

  const body = renderMiniMarkdown(note.body);

  card.append(header, body);

  editButton.addEventListener('click', () => card.replaceWith(createEditForm(note)));
  deleteButton.addEventListener('click', async () => {
    try {
      await deleteNote(note.id);
      card.remove();
    } catch (error) {
      console.error('Failed to delete note:', error);
    }
  });

  return card;
}

function createEditForm(note) {
  const form = document.createElement('form');
  form.className = 'flex flex-col gap-2 rounded-xl border border-emerald-500/30 bg-slate-800 p-5';

  const titleInput = document.createElement('input');
  titleInput.type = 'text';
  titleInput.value = note.title ?? '';
  titleInput.placeholder = 'Title (optional)';
  titleInput.className =
    'rounded-lg border border-white/10 bg-slate-900 px-3 py-2 text-sm text-slate-50 placeholder:text-slate-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  titleInput.setAttribute('aria-label', 'Note title (optional)');

  const bodyInput = document.createElement('textarea');
  bodyInput.rows = 4;
  bodyInput.required = true;
  bodyInput.value = note.body;
  bodyInput.className =
    'rounded-lg border border-white/10 bg-slate-900 px-3 py-2 text-sm text-slate-50 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  bodyInput.setAttribute('aria-label', 'Note body');

  const actions = document.createElement('div');
  actions.className = 'flex justify-end gap-2';

  const cancelButton = document.createElement('button');
  cancelButton.type = 'button';
  cancelButton.className = 'rounded-lg border border-white/10 px-3 py-1.5 text-sm text-slate-300 hover:text-slate-50 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  cancelButton.textContent = 'Cancel';
  cancelButton.addEventListener('click', () => form.replaceWith(createNoteCard(note)));

  const saveButton = document.createElement('button');
  saveButton.type = 'submit';
  saveButton.className =
    'rounded-lg border border-emerald-500/40 bg-emerald-500/10 px-3 py-1.5 text-sm font-medium text-emerald-400 hover:bg-emerald-500/20 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  saveButton.textContent = 'Save';

  actions.append(cancelButton, saveButton);
  form.append(titleInput, bodyInput, actions);

  form.addEventListener('submit', async (event) => {
    event.preventDefault();
    const body = bodyInput.value.trim();
    if (!body) return;

    try {
      const updated = await updateNote(note.id, { title: titleInput.value.trim() || null, body });
      form.replaceWith(createNoteCard({ ...note, ...updated }));
    } catch (error) {
      console.error('Failed to update note:', error);
    }
  });

  return form;
}
