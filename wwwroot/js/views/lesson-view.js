import { getLesson, setLessonCompleted } from '../api/lessons.js';
import { getProgressSummary } from '../api/progress.js';
import { createBookmark, deleteBookmark } from '../api/bookmarks.js';
import { createBreadcrumb } from '../components/breadcrumb.js';
import { renderContentBlock } from '../components/content-blocks/index.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';
import { setState } from '../state.js';

// Route: /lesson/:lessonSlug
export async function renderLessonView(params, query, root) {
  root.replaceChildren(createLoadingMessage('Loading lesson…'));
  const controller = new AbortController();

  try {
    const lesson = await getLesson(params.lessonSlug, { signal: controller.signal });
    document.title = `${lesson.title} · Mentor OS`;
    root.replaceChildren(buildLessonView(lesson));
  } catch (error) {
    if (error.name === 'AbortError') return;
    document.title = 'Lesson Not Found · Mentor OS';
    root.replaceChildren(createErrorMessage(error, 'That lesson could not be found.'));
  }

  return () => controller.abort();
}

function buildLessonView(lesson) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-6';

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

  const actions = document.createElement('div');
  actions.className = 'flex shrink-0 items-center gap-2';
  actions.append(createBookmarkToggle(lesson), createCompleteToggle(lesson));

  titleCol.append(heading, summary);
  titleRow.append(titleCol, actions);
  header.appendChild(titleRow);

  const blocksContainer = document.createElement('div');
  blocksContainer.className = 'flex flex-col gap-4';
  lesson.contentBlocks.forEach((block) => blocksContainer.appendChild(renderContentBlock(block)));

  container.append(header, blocksContainer);
  return container;
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
