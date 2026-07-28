import { setChecklistItemCompleted } from '../api/checklists.js';

// Reused for Lesson, Capstone, and Project checklists — all backed by the
// same polymorphic ChecklistItem completion endpoint.
export function createChecklist(items) {
  const list = document.createElement('ul');
  list.className = 'flex flex-col gap-2';

  items.forEach((item) => list.appendChild(createChecklistRow(item)));

  return list;
}

function createChecklistRow(item) {
  const row = document.createElement('li');
  row.className = 'flex items-start gap-3 text-sm';

  const label = document.createElement('label');
  label.className = 'flex flex-1 cursor-pointer items-start gap-3';

  const checkbox = document.createElement('input');
  checkbox.type = 'checkbox';
  checkbox.checked = item.isCompleted;
  checkbox.className = 'mt-0.5 h-4 w-4 rounded accent-emerald-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  checkbox.setAttribute('aria-label', item.description);

  const text = document.createElement('span');
  text.className = item.isCompleted ? 'text-slate-500 line-through' : 'text-slate-300';
  text.textContent = item.description;

  checkbox.addEventListener('change', async () => {
    const wasChecked = !checkbox.checked;
    try {
      await setChecklistItemCompleted(item.id, checkbox.checked);
      text.className = checkbox.checked ? 'text-slate-500 line-through' : 'text-slate-300';
    } catch (error) {
      checkbox.checked = wasChecked;
      console.error('Failed to update checklist item:', error);
    }
  });

  label.append(checkbox, text);
  row.appendChild(label);
  return row;
}
