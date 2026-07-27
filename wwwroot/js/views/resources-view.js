import { getResources, setResourceCompleted } from '../api/resources.js';
import { getProgressSummary } from '../api/progress.js';
import { createResourceCard, getCardClassName } from '../components/resource-card.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';
import { setState } from '../state.js';

export async function renderResourcesView(params, query, root) {
  document.title = 'Resources · Mentor OS';
  root.replaceChildren(createLoadingMessage('Loading resources…'));

  const controller = new AbortController();

  try {
    const resources = await getResources({ signal: controller.signal });
    root.replaceChildren(buildView(resources));
  } catch (error) {
    if (error.name === 'AbortError') return;
    root.replaceChildren(createErrorMessage(error, 'Something went wrong loading resources.'));
  }

  return () => controller.abort();
}

function buildView(resources) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-6';

  const heading = document.createElement('h1');
  heading.className = 'text-2xl font-semibold tracking-tight text-slate-50';
  heading.textContent = 'Supplementary Resources';

  const subheading = document.createElement('p');
  subheading.className = 'text-sm text-slate-400';
  subheading.textContent = 'External links that complement the in-app curriculum.';

  const grid = document.createElement('div');
  grid.className = 'grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3';
  resources.forEach((resource) => grid.appendChild(createResourceCard(resource)));

  container.append(heading, subheading, grid);
  // One delegated listener handles every checkbox, present and future,
  // instead of attaching one listener per card.
  container.addEventListener('change', handleToggle);

  return container;
}

async function handleToggle(event) {
  const checkbox = event.target;
  if (!checkbox.matches('input[type="checkbox"][data-resource-id]')) return;

  const resourceId = Number(checkbox.dataset.resourceId);
  const card = checkbox.closest('article');
  const wasChecked = !checkbox.checked;

  try {
    await setResourceCompleted(resourceId, checkbox.checked);
    card.className = getCardClassName(checkbox.checked);
    setState('progress', await getProgressSummary());
  } catch (error) {
    checkbox.checked = wasChecked;
    console.error('Failed to update resource completion:', error);
  }
}
