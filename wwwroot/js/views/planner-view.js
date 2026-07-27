import { getPlanItemsForDate, createPlanItem, setPlanItemDone, deletePlanItem } from '../api/planner.js';
import { getCurrentStreak } from '../api/streaks.js';
import { getProgressSummary } from '../api/progress.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';
import { toDateKey, addDays, formatDateLabel } from '../utils/format.js';
import { navigate, refresh } from '../router.js';
import { setState } from '../state.js';

// Route: /planner (optionally ?date=yyyy-MM-dd, defaults to today)
export async function renderPlannerView(params, query, root) {
  const date = query.get('date') || toDateKey(new Date());
  document.title = 'Planner · Mentor OS';
  root.replaceChildren(createLoadingMessage('Loading planner…'));

  const controller = new AbortController();

  try {
    const [items, streak] = await Promise.all([
      getPlanItemsForDate(date, { signal: controller.signal }),
      getCurrentStreak({ signal: controller.signal }),
    ]);
    root.replaceChildren(buildView(date, items, streak));
  } catch (error) {
    if (error.name === 'AbortError') return;
    root.replaceChildren(createErrorMessage(error, 'Something went wrong loading the planner.'));
  }

  return () => controller.abort();
}

function buildView(date, items, streak) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-6';

  const header = document.createElement('header');
  header.className = 'flex flex-wrap items-center justify-between gap-4';

  const titleCol = document.createElement('div');
  const heading = document.createElement('h1');
  heading.className = 'text-2xl font-semibold tracking-tight text-slate-50';
  heading.textContent = 'Daily Planner';

  const dateLabel = document.createElement('p');
  dateLabel.className = 'text-sm text-slate-400';
  dateLabel.textContent = formatDateLabel(date);

  titleCol.append(heading, dateLabel);

  const streakBadge = document.createElement('div');
  streakBadge.className =
    'flex items-center gap-2 rounded-lg border border-emerald-500/20 bg-emerald-500/5 px-4 py-2 text-sm text-emerald-400';
  streakBadge.textContent = `🔥 ${streak.currentStreakDays}-day streak · best ${streak.longestStreakDays}`;

  header.append(titleCol, streakBadge);

  const dateNav = document.createElement('div');
  dateNav.className = 'flex items-center gap-2';
  dateNav.append(
    createDateNavButton('← Previous day', addDays(date, -1)),
    createDateNavButton('Next day →', addDays(date, 1)),
  );

  const list = document.createElement('div');
  list.className = 'flex flex-col gap-3';

  if (items.length === 0) {
    const empty = document.createElement('p');
    empty.className = 'text-sm text-slate-400';
    empty.textContent = 'Nothing planned for this day yet.';
    list.appendChild(empty);
  } else {
    items.forEach((item) => list.appendChild(createPlanItemRow(item)));
  }

  container.append(header, dateNav, list, createAddItemForm(date));
  return container;
}

function createDateNavButton(label, targetDate) {
  const button = document.createElement('button');
  button.type = 'button';
  button.className =
    'rounded-lg border border-white/10 px-3 py-1.5 text-xs font-medium text-slate-300 transition duration-300 hover:border-emerald-500/40 hover:text-emerald-400 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  button.textContent = label;
  button.addEventListener('click', () => navigate(`/planner?date=${targetDate}`));
  return button;
}

function createPlanItemRow(item) {
  const row = document.createElement('div');
  row.className =
    'flex items-center justify-between gap-4 rounded-xl border border-white/5 bg-slate-800 p-4 shadow-lg shadow-black/20';

  const label = document.createElement('label');
  label.className = 'flex flex-1 cursor-pointer items-center gap-3 text-sm';

  const checkbox = document.createElement('input');
  checkbox.type = 'checkbox';
  checkbox.checked = item.isDone;
  checkbox.className = 'h-4 w-4 rounded accent-emerald-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  checkbox.setAttribute('aria-label', `Mark ${item.displayTitle} as done`);

  const title = document.createElement('span');
  title.className = item.isDone ? 'text-slate-500 line-through' : 'text-slate-200';
  title.textContent = item.displayTitle;

  checkbox.addEventListener('change', async () => {
    const wasChecked = !checkbox.checked;
    try {
      await setPlanItemDone(item.id, checkbox.checked);
      title.className = checkbox.checked ? 'text-slate-500 line-through' : 'text-slate-200';
      setState('progress', await getProgressSummary());
    } catch (error) {
      checkbox.checked = wasChecked;
      console.error('Failed to update plan item:', error);
    }
  });

  label.append(checkbox, title);

  const removeButton = document.createElement('button');
  removeButton.type = 'button';
  removeButton.className =
    'rounded text-xs text-slate-500 hover:text-red-400 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  removeButton.textContent = 'Remove';
  removeButton.setAttribute('aria-label', `Remove ${item.displayTitle} from planner`);
  removeButton.addEventListener('click', async () => {
    try {
      await deletePlanItem(item.id);
      row.remove();
    } catch (error) {
      console.error('Failed to delete plan item:', error);
    }
  });

  row.append(label, removeButton);
  return row;
}

function createAddItemForm(date) {
  const form = document.createElement('form');
  form.className = 'flex gap-2';

  const input = document.createElement('input');
  input.type = 'text';
  input.placeholder = 'Add a task for today…';
  input.required = true;
  input.className =
    'flex-1 rounded-lg border border-white/10 bg-slate-800 px-3 py-2 text-sm text-slate-50 placeholder:text-slate-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  input.setAttribute('aria-label', 'New planner task');

  const submitButton = document.createElement('button');
  submitButton.type = 'submit';
  submitButton.className =
    'rounded-lg border border-emerald-500/40 bg-emerald-500/10 px-4 py-2 text-sm font-medium text-emerald-400 transition duration-300 hover:bg-emerald-500/20 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  submitButton.textContent = 'Add';

  form.append(input, submitButton);

  form.addEventListener('submit', async (event) => {
    event.preventDefault();
    const title = input.value.trim();
    if (!title) return;

    try {
      await createPlanItem({ planDate: date, entityKind: 'Custom', customTitle: title, sortOrder: 0 });
      await refresh();
    } catch (error) {
      console.error('Failed to add plan item:', error);
    }
  });

  return form;
}
