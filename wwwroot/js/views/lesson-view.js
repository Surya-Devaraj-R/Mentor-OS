import { getLesson, setLessonCompleted } from '../api/lessons.js';
import { getProgressSummary } from '../api/progress.js';
import { createBookmark, deleteBookmark } from '../api/bookmarks.js';
import { getNotes, createNote } from '../api/notes.js';
import { createBreadcrumb } from '../components/breadcrumb.js';
import { renderContentBlock } from '../components/content-blocks/index.js';
import { renderMiniMarkdown } from '../components/content-blocks/mini-markdown.js';
import { createChecklist } from '../components/checklist.js';
import { createQuiz } from '../components/quiz.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';
import { setState } from '../state.js';

// Route: /lesson/:lessonSlug
export async function renderLessonView(params, query, root) {
  root.replaceChildren(createLoadingMessage('Loading lesson…'));
  const controller = new AbortController();

  try {
    const lesson = await getLesson(params.lessonSlug, { signal: controller.signal });
    const lessonNotes = await getNotes({ lessonId: lesson.id }, { signal: controller.signal }).catch(() => []);
    document.title = `${lesson.title} · Mentor OS`;
    root.replaceChildren(buildLessonView(lesson, lessonNotes));
  } catch (error) {
    if (error.name === 'AbortError') return;
    document.title = 'Lesson Not Found · Mentor OS';
    root.replaceChildren(createErrorMessage(error, 'That lesson could not be found.'));
  }

  return () => controller.abort();
}

function buildLessonView(lesson, lessonNotes) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-8';

  container.appendChild(buildHeader(lesson));

  if (lesson.objectives.length > 0) {
    container.appendChild(buildObjectives(lesson.objectives));
  }

  const blocksContainer = document.createElement('div');
  blocksContainer.className = 'flex flex-col gap-4';
  lesson.contentBlocks.forEach((block) => blocksContainer.appendChild(renderContentBlock(block)));
  container.appendChild(blocksContainer);

  if (lesson.quiz.length > 0) {
    container.appendChild(buildSection('Quick Check', createQuiz(lesson.quiz)));
  }

  if (lesson.checklist.length > 0) {
    container.appendChild(buildSection('Before You Move On', createChecklist(lesson.checklist)));
  }

  if (lesson.referenceLinks.length > 0) {
    container.appendChild(buildSection('Further Reading & Docs', createReferenceLinks(lesson.referenceLinks)));
  }

  container.appendChild(buildNotesSection(lesson, lessonNotes));

  return container;
}

function buildHeader(lesson) {
  const header = document.createElement('header');
  header.className = 'flex flex-col gap-3';
  header.appendChild(createBreadcrumb([{ label: 'Dashboard', hash: '#/' }, { label: lesson.title }]));

  const titleRow = document.createElement('div');
  titleRow.className = 'flex flex-wrap items-start justify-between gap-4';

  const titleCol = document.createElement('div');
  titleCol.className = 'flex flex-col gap-1';

  const heading = document.createElement('h1');
  heading.className = 'text-2xl font-semibold tracking-tight text-slate-50';
  heading.textContent = lesson.title;

  const summary = document.createElement('p');
  summary.className = 'text-sm text-slate-400';
  summary.textContent = lesson.summary;

  titleCol.append(heading, summary);

  if (lesson.prerequisites.length > 0) {
    titleCol.appendChild(createPrerequisites(lesson.prerequisites));
  }

  const actions = document.createElement('div');
  actions.className = 'flex shrink-0 items-center gap-2';
  actions.append(createBookmarkToggle(lesson), createCompleteToggle(lesson));

  titleRow.append(titleCol, actions);
  header.appendChild(titleRow);
  return header;
}

function createPrerequisites(prerequisites) {
  const wrapper = document.createElement('p');
  wrapper.className = 'text-xs text-slate-500';

  const label = document.createElement('span');
  label.textContent = 'Recommended before this: ';
  wrapper.appendChild(label);

  prerequisites.forEach((prereq, index) => {
    if (index > 0) wrapper.appendChild(document.createTextNode(', '));
    const link = document.createElement('a');
    link.href = `#/lesson/${prereq.slug}`;
    link.className = 'text-emerald-400 hover:underline';
    link.textContent = prereq.title;
    wrapper.appendChild(link);
  });

  return wrapper;
}

function buildObjectives(objectives) {
  const section = document.createElement('section');
  section.className = 'flex flex-col gap-2 rounded-xl border border-emerald-500/20 bg-emerald-500/5 p-5';
  section.setAttribute('aria-label', "What You'll Learn");

  const heading = document.createElement('h2');
  heading.className = 'text-xs font-semibold uppercase tracking-wide text-emerald-400';
  heading.textContent = "What You'll Learn";

  const list = document.createElement('ul');
  list.className = 'list-disc space-y-1 pl-5 text-sm text-slate-300';
  objectives.forEach((text) => {
    const item = document.createElement('li');
    item.textContent = text;
    list.appendChild(item);
  });

  section.append(heading, list);
  return section;
}

function buildSection(title, content) {
  const section = document.createElement('section');
  section.className = 'flex flex-col gap-3';

  const heading = document.createElement('h2');
  heading.className = 'text-lg font-semibold tracking-tight text-slate-50';
  heading.textContent = title;

  section.append(heading, content);
  return section;
}

function createReferenceLinks(links) {
  const list = document.createElement('ul');
  list.className = 'flex flex-col gap-2';

  links.forEach((link) => {
    const item = document.createElement('li');
    const anchor = document.createElement('a');
    anchor.href = link.url;
    anchor.target = '_blank';
    anchor.rel = 'noopener noreferrer';
    anchor.className =
      'inline-flex items-center gap-2 rounded-lg border border-white/10 px-3 py-2 text-sm text-slate-300 transition duration-300 hover:border-emerald-500/40 hover:text-emerald-400 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';

    const badge = document.createElement('span');
    badge.className = 'rounded bg-slate-900 px-1.5 py-0.5 text-xs uppercase text-slate-500';
    badge.textContent = link.linkType === 'OfficialDocs' ? 'Docs' : 'Reading';

    anchor.append(badge, document.createTextNode(link.title));
    const srOnly = document.createElement('span');
    srOnly.className = 'sr-only';
    srOnly.textContent = ' (opens in a new tab)';
    anchor.appendChild(srOnly);

    item.appendChild(anchor);
    list.appendChild(item);
  });

  return list;
}

function buildNotesSection(lesson, lessonNotes) {
  const section = document.createElement('section');
  section.className = 'flex flex-col gap-3';

  const heading = document.createElement('h2');
  heading.className = 'text-lg font-semibold tracking-tight text-slate-50';
  heading.textContent = 'Your Notes';

  const list = document.createElement('div');
  list.className = 'flex flex-col gap-2';
  lessonNotes.forEach((note) => {
    const card = document.createElement('div');
    card.className = 'rounded-lg border border-white/5 bg-slate-800 p-3';
    card.appendChild(renderMiniMarkdown(note.body));
    list.appendChild(card);
  });

  const form = document.createElement('form');
  form.className = 'flex gap-2';

  const input = document.createElement('textarea');
  input.rows = 2;
  input.required = true;
  input.placeholder = 'Add a note for this lesson…';
  input.className =
    'flex-1 rounded-lg border border-white/10 bg-slate-900 px-3 py-2 text-sm text-slate-50 placeholder:text-slate-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  input.setAttribute('aria-label', 'New note for this lesson');

  const submitButton = document.createElement('button');
  submitButton.type = 'submit';
  submitButton.className =
    'self-start rounded-lg border border-emerald-500/40 bg-emerald-500/10 px-4 py-2 text-sm font-medium text-emerald-400 transition duration-300 hover:bg-emerald-500/20 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  submitButton.textContent = 'Add';

  form.append(input, submitButton);

  form.addEventListener('submit', async (event) => {
    event.preventDefault();
    const body = input.value.trim();
    if (!body) return;

    try {
      const note = await createNote({ lessonId: lesson.id, title: null, body });
      const card = document.createElement('div');
      card.className = 'rounded-lg border border-white/5 bg-slate-800 p-3';
      card.appendChild(renderMiniMarkdown(note.body));
      list.appendChild(card);
      input.value = '';
    } catch (error) {
      console.error('Failed to add note:', error);
    }
  });

  section.append(heading, list, form);
  return section;
}

function createCompleteToggle(lesson) {
  const label = document.createElement('label');
  label.className =
    'flex shrink-0 cursor-pointer select-none items-center gap-2 rounded-lg border border-white/10 px-4 py-2 text-sm font-medium text-slate-50';

  const checkbox = document.createElement('input');
  checkbox.type = 'checkbox';
  checkbox.checked = lesson.isCompleted;
  checkbox.className = 'h-4 w-4 rounded accent-emerald-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  checkbox.setAttribute('aria-label', `Mark ${lesson.title} as completed`);

  checkbox.addEventListener('change', async () => {
    const wasChecked = !checkbox.checked;
    try {
      await setLessonCompleted(lesson.id, checkbox.checked);
      setState('progress', await getProgressSummary());
    } catch (error) {
      checkbox.checked = wasChecked;
      console.error('Failed to update lesson completion:', error);
    }
  });

  label.append(checkbox, document.createTextNode('Mark complete'));
  return label;
}

function createBookmarkToggle(lesson) {
  let bookmarkId = lesson.bookmarkId;

  const button = document.createElement('button');
  button.type = 'button';
  button.className =
    'rounded-lg border border-white/10 px-3 py-2 text-sm text-slate-300 transition duration-300 hover:border-emerald-500/40 hover:text-emerald-400 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';

  const updateLabel = () => {
    button.textContent = bookmarkId ? '★ Bookmarked' : '☆ Bookmark';
    button.setAttribute('aria-pressed', String(Boolean(bookmarkId)));
    button.setAttribute('aria-label', bookmarkId ? `Remove bookmark for ${lesson.title}` : `Bookmark ${lesson.title}`);
  };
  updateLabel();

  button.addEventListener('click', async () => {
    try {
      if (bookmarkId) {
        await deleteBookmark(bookmarkId);
        bookmarkId = null;
      } else {
        const created = await createBookmark({ entityKind: 'Lesson', entityId: lesson.id });
        bookmarkId = created.id;
      }
      updateLabel();
    } catch (error) {
      console.error('Failed to toggle bookmark:', error);
    }
  });

  return button;
}
