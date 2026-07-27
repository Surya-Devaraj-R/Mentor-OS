import { getTopic } from '../api/topics.js';
import { getModulesForTopic, getModule } from '../api/modules.js';
import { createBreadcrumb } from '../components/breadcrumb.js';
import { renderMiniMarkdown } from '../components/content-blocks/mini-markdown.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';
import { navigate } from '../router.js';

// Route: /roadmap/:topicSlug — lists the modules within one topic.
export async function renderTopicView(params, query, root) {
  root.replaceChildren(createLoadingMessage('Loading topic…'));
  const controller = new AbortController();

  try {
    const [topic, modules] = await Promise.all([
      getTopic(params.topicSlug, { signal: controller.signal }),
      getModulesForTopic(params.topicSlug, { signal: controller.signal }),
    ]);
    document.title = `${topic.title} · Mentor OS`;
    root.replaceChildren(buildTopicView(topic, modules));
  } catch (error) {
    if (error.name === 'AbortError') return;
    document.title = 'Topic Not Found · Mentor OS';
    root.replaceChildren(createErrorMessage(error, 'That topic could not be found.'));
  }

  return () => controller.abort();
}

function buildTopicView(topic, modules) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-6';

  const header = document.createElement('header');
  header.className = 'flex flex-col gap-3';
  header.append(
    createBreadcrumb([{ label: 'Dashboard', hash: '#/' }, { label: topic.title }]),
  );

  const heading = document.createElement('h1');
  heading.className = 'text-2xl font-semibold tracking-tight text-slate-50';
  heading.textContent = topic.title;

  const description = document.createElement('p');
  description.className = 'text-sm text-slate-400';
  description.textContent = topic.description;

  header.append(heading, description);

  const grid = document.createElement('div');
  grid.className = 'grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3';
  modules.forEach((module) => grid.appendChild(createModuleCard(module, topic.slug)));

  container.append(header, grid);
  return container;
}

function createModuleCard(module, topicSlug) {
  const card = document.createElement('button');
  card.type = 'button';
  card.className =
    'flex flex-col gap-2 rounded-xl border border-white/5 bg-slate-800 p-5 text-left shadow-lg shadow-black/20 transition duration-300 hover:-translate-y-1 hover:border-emerald-500/30 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';

  const title = document.createElement('h3');
  title.className = 'text-base font-semibold text-slate-50';
  title.textContent = module.title;

  const description = document.createElement('p');
  description.className = 'text-sm text-slate-400';
  description.textContent = module.description;

  card.append(title, description);

  if (module.estimatedMinutes) {
    const meta = document.createElement('span');
    meta.className = 'text-xs text-slate-500';
    meta.textContent = `~${module.estimatedMinutes} min`;
    card.appendChild(meta);
  }

  card.addEventListener('click', () => navigate(`/roadmap/${topicSlug}/${module.slug}`));
  return card;
}

// Route: /roadmap/:topicSlug/:moduleSlug — lists the lessons (and capstone,
// if any) within one module.
export async function renderModuleView(params, query, root) {
  root.replaceChildren(createLoadingMessage('Loading module…'));
  const controller = new AbortController();

  try {
    const [topic, module] = await Promise.all([
      getTopic(params.topicSlug, { signal: controller.signal }),
      getModule(params.moduleSlug, { signal: controller.signal }),
    ]);
    document.title = `${module.title} · Mentor OS`;
    root.replaceChildren(buildModuleView(topic, module));
  } catch (error) {
    if (error.name === 'AbortError') return;
    document.title = 'Module Not Found · Mentor OS';
    root.replaceChildren(createErrorMessage(error, 'That module could not be found.'));
  }

  return () => controller.abort();
}

function buildModuleView(topic, module) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-6';

  const header = document.createElement('header');
  header.className = 'flex flex-col gap-3';
  header.append(
    createBreadcrumb([
      { label: 'Dashboard', hash: '#/' },
      { label: topic.title, hash: `#/roadmap/${topic.slug}` },
      { label: module.title },
    ]),
  );

  const heading = document.createElement('h1');
  heading.className = 'text-2xl font-semibold tracking-tight text-slate-50';
  heading.textContent = module.title;

  const description = document.createElement('p');
  description.className = 'text-sm text-slate-400';
  description.textContent = module.description;

  header.append(heading, description);

  const lessonsHeading = document.createElement('h2');
  lessonsHeading.className = 'text-lg font-semibold tracking-tight text-slate-50';
  lessonsHeading.textContent = 'Lessons';

  const lessonList = document.createElement('div');
  lessonList.className = 'flex flex-col gap-3';
  module.lessons.forEach((lesson) => lessonList.appendChild(createLessonRow(lesson)));

  container.append(header, lessonsHeading, lessonList);

  if (module.capstone) {
    container.appendChild(createCapstoneCard(module.capstone));
  }

  return container;
}

function createLessonRow(lesson) {
  const row = document.createElement('button');
  row.type = 'button';
  row.className =
    'flex items-center justify-between gap-4 rounded-xl border border-white/5 bg-slate-800 p-5 text-left shadow-lg shadow-black/20 transition duration-300 hover:-translate-y-1 hover:border-emerald-500/30 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';

  const textCol = document.createElement('div');
  textCol.className = 'flex flex-col gap-1';

  const title = document.createElement('h3');
  title.className = 'text-base font-semibold text-slate-50';
  title.textContent = lesson.title;

  const summary = document.createElement('p');
  summary.className = 'text-sm text-slate-400';
  summary.textContent = lesson.summary;

  textCol.append(title, summary);

  const meta = document.createElement('div');
  meta.className = 'flex shrink-0 items-center gap-3 text-xs text-slate-500';

  if (lesson.estimatedMinutes) {
    const time = document.createElement('span');
    time.textContent = `~${lesson.estimatedMinutes} min`;
    meta.appendChild(time);
  }

  if (lesson.isCompleted) {
    const badge = document.createElement('span');
    badge.className = 'rounded-full bg-emerald-500/10 px-2 py-1 font-medium text-emerald-400';
    badge.textContent = '✓ Completed';
    meta.appendChild(badge);
  }

  row.append(textCol, meta);
  row.addEventListener('click', () => navigate(`/lesson/${lesson.slug}`));
  return row;
}

function createCapstoneCard(capstone) {
  const card = document.createElement('section');
  card.className = 'flex flex-col gap-4 rounded-xl border border-emerald-500/20 bg-emerald-500/5 p-6';
  card.setAttribute('aria-label', 'Capstone project');

  const eyebrow = document.createElement('span');
  eyebrow.className = 'text-xs font-semibold uppercase tracking-wide text-emerald-400';
  eyebrow.textContent = 'Capstone Project';

  const title = document.createElement('h2');
  title.className = 'text-lg font-semibold text-slate-50';
  title.textContent = capstone.title;

  const description = renderMiniMarkdown(capstone.description);

  const requirementsHeading = document.createElement('h3');
  requirementsHeading.className = 'text-xs font-semibold uppercase tracking-wide text-slate-400';
  requirementsHeading.textContent = 'Requirements';

  const requirements = renderMiniMarkdown(capstone.requirements);

  const checklistHeading = document.createElement('h3');
  checklistHeading.className = 'text-xs font-semibold uppercase tracking-wide text-slate-400';
  checklistHeading.textContent = 'Checklist';

  const checklist = document.createElement('ul');
  checklist.className = 'flex flex-col gap-1.5 text-sm text-slate-300';
  capstone.checklistItems.forEach((item) => {
    const li = document.createElement('li');
    li.className = 'flex items-start gap-2';

    const marker = document.createElement('span');
    marker.setAttribute('aria-hidden', 'true');
    marker.textContent = '☐';
    li.append(marker, document.createTextNode(item.description));

    checklist.appendChild(li);
  });

  card.append(eyebrow, title, description, requirementsHeading, requirements, checklistHeading, checklist);
  return card;
}
