import { getTopic } from '../api/topics.js';
import { getModulesForTopic, getModule } from '../api/modules.js';
import { getProjectForTopic } from '../api/projects.js';
import { createBreadcrumb } from '../components/breadcrumb.js';
import { renderMiniMarkdown } from '../components/content-blocks/mini-markdown.js';
import { createChecklist } from '../components/checklist.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';
import { navigate } from '../router.js';
import { getTopicIcon } from '../utils/icons.js';

// Route: /roadmap/:topicSlug — lists the modules within one topic.
export async function renderTopicView(params, query, root) {
  root.replaceChildren(createLoadingMessage('Loading topic…'));
  const controller = new AbortController();

  try {
    const [topic, modules, project] = await Promise.all([
      getTopic(params.topicSlug, { signal: controller.signal }),
      getModulesForTopic(params.topicSlug, { signal: controller.signal }),
      getProjectForTopic(params.topicSlug, { signal: controller.signal }).catch(() => null),
    ]);
    document.title = `${topic.title} · Mentor OS`;
    root.replaceChildren(buildTopicView(topic, modules, project));
  } catch (error) {
    if (error.name === 'AbortError') return;
    document.title = 'Topic Not Found · Mentor OS';
    root.replaceChildren(createErrorMessage(error, 'That topic could not be found.'));
  }

  return () => controller.abort();
}

function buildTopicView(topic, modules, project) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-6 animate-fade-in-up';

  const header = document.createElement('header');
  header.className = 'flex flex-col gap-3';
  header.append(
    createBreadcrumb([{ label: 'Dashboard', hash: '#/' }, { label: topic.title }]),
  );

  const headingRow = document.createElement('div');
  headingRow.className = 'flex items-center gap-3';

  const iconBadge = document.createElement('span');
  iconBadge.className =
    'flex h-12 w-12 shrink-0 items-center justify-center rounded-2xl bg-gradient-to-br from-emerald-500/15 via-slate-800 to-violet-500/15 text-2xl ring-1 ring-white/10';
  iconBadge.setAttribute('aria-hidden', 'true');
  iconBadge.textContent = getTopicIcon(topic);

  const heading = document.createElement('h1');
  heading.className = 'text-2xl font-extrabold tracking-tight text-slate-50 sm:text-3xl';
  heading.textContent = topic.title;

  headingRow.append(iconBadge, heading);

  const description = document.createElement('p');
  description.className = 'text-sm text-slate-400';
  description.textContent = topic.description;

  const moduleCountPill = document.createElement('span');
  moduleCountPill.className =
    'inline-flex w-fit items-center gap-1 rounded-full border border-white/10 bg-slate-800/60 px-3 py-1 text-xs font-medium text-slate-400';
  moduleCountPill.textContent = `${modules.length} module${modules.length === 1 ? '' : 's'}`;

  header.append(headingRow, description, moduleCountPill);

  const grid = document.createElement('div');
  grid.className = 'grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3';
  modules.forEach((module, index) => grid.appendChild(createModuleCard(module, topic.slug, index + 1)));

  container.append(header, grid);

  if (project) {
    container.appendChild(createProjectCard(project, topic.slug));
  }

  return container;
}

function createProjectCard(project, topicSlug) {
  const card = document.createElement('button');
  card.type = 'button';
  card.className =
    'group flex flex-col gap-2 rounded-2xl border border-emerald-500/20 bg-gradient-to-br from-emerald-500/10 to-violet-500/10 p-6 text-left shadow-lg shadow-black/10 transition duration-300 hover:-translate-y-1 hover:border-emerald-500/40 hover:shadow-[0_8px_30px_rgba(16,185,129,0.15),0_10px_40px_rgba(139,92,246,0.12)] focus:outline-none focus:ring-2 focus:ring-emerald-500/50';

  const eyebrow = document.createElement('span');
  eyebrow.className = 'flex items-center gap-1.5 text-xs font-semibold uppercase tracking-wide text-emerald-400';
  eyebrow.textContent = '🚀 Production Project';

  const title = document.createElement('h3');
  title.className = 'text-base font-semibold text-slate-50';
  title.textContent = project.title;

  const description = document.createElement('p');
  description.className = 'text-sm text-slate-400';
  description.textContent = project.description;

  card.append(eyebrow, title, description);
  card.addEventListener('click', () => navigate(`/projects/${topicSlug}`));
  return card;
}

function createModuleCard(module, topicSlug, position) {
  const card = document.createElement('button');
  card.type = 'button';
  card.className =
    'group flex flex-col gap-3 rounded-2xl border border-white/5 bg-slate-800/60 p-5 text-left shadow-lg shadow-black/20 ring-1 ring-white/5 backdrop-blur transition duration-300 hover:-translate-y-1 hover:border-emerald-500/30 hover:shadow-[0_8px_30px_rgba(16,185,129,0.15),0_10px_40px_rgba(139,92,246,0.12)] focus:outline-none focus:ring-2 focus:ring-emerald-500/50';

  const topRow = document.createElement('div');
  topRow.className = 'flex items-start justify-between gap-3';

  const badge = document.createElement('span');
  badge.className =
    'flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-emerald-500/10 text-xs font-bold text-emerald-400 ring-1 ring-emerald-500/20';
  badge.textContent = String(position).padStart(2, '0');
  badge.setAttribute('aria-hidden', 'true');

  const arrow = document.createElement('span');
  arrow.className =
    'text-slate-600 opacity-0 transition duration-300 group-hover:translate-x-0.5 group-hover:text-emerald-400 group-hover:opacity-100';
  arrow.setAttribute('aria-hidden', 'true');
  arrow.textContent = '→';

  topRow.append(badge, arrow);

  const title = document.createElement('h3');
  title.className = 'text-base font-semibold text-slate-50';
  title.textContent = module.title;

  const description = document.createElement('p');
  description.className = 'text-sm text-slate-400';
  description.textContent = module.description;

  card.append(topRow, title, description);

  if (module.estimatedMinutes) {
    const meta = document.createElement('span');
    meta.className =
      'inline-flex w-fit items-center gap-1 rounded-full bg-slate-900/60 px-2.5 py-1 text-xs font-medium text-slate-400';
    meta.textContent = `🕒 ~${module.estimatedMinutes} min`;
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
  container.className = 'flex flex-col gap-6 animate-fade-in-up';

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
  heading.className = 'text-2xl font-extrabold tracking-tight text-slate-50 sm:text-3xl';
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
  module.lessons.forEach((lesson, index) => lessonList.appendChild(createLessonRow(lesson, index + 1)));

  container.append(header, lessonsHeading, lessonList);

  if (module.capstone) {
    container.appendChild(createCapstoneCard(module.capstone));
  }

  return container;
}

function createLessonRow(lesson, position) {
  const row = document.createElement('button');
  row.type = 'button';
  row.className =
    'group flex items-center justify-between gap-4 rounded-2xl border border-white/5 bg-slate-800/60 p-5 text-left shadow-lg shadow-black/20 ring-1 ring-white/5 backdrop-blur transition duration-300 hover:-translate-y-1 hover:border-emerald-500/30 hover:shadow-[0_8px_30px_rgba(16,185,129,0.15),0_10px_40px_rgba(139,92,246,0.12)] focus:outline-none focus:ring-2 focus:ring-emerald-500/50';

  const leftGroup = document.createElement('div');
  leftGroup.className = 'flex items-center gap-4';

  const marker = document.createElement('span');
  marker.setAttribute('aria-hidden', 'true');
  if (lesson.isCompleted) {
    marker.className =
      'flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-emerald-500 text-sm font-bold text-slate-900 shadow shadow-emerald-500/30';
    marker.textContent = '✓';
  } else {
    marker.className =
      'flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-slate-900/60 text-xs font-bold text-slate-500 ring-1 ring-white/10';
    marker.textContent = String(position).padStart(2, '0');
  }

  const textCol = document.createElement('div');
  textCol.className = 'flex flex-col gap-1';

  const title = document.createElement('h3');
  title.className = 'text-base font-semibold text-slate-50';
  title.textContent = lesson.title;

  const summary = document.createElement('p');
  summary.className = 'text-sm text-slate-400';
  summary.textContent = lesson.summary;

  textCol.append(title, summary);
  leftGroup.append(marker, textCol);

  const meta = document.createElement('div');
  meta.className = 'flex shrink-0 items-center gap-3 text-xs text-slate-500';

  if (lesson.estimatedMinutes) {
    const time = document.createElement('span');
    time.textContent = `🕒 ~${lesson.estimatedMinutes} min`;
    meta.appendChild(time);
  }

  if (lesson.isCompleted) {
    const badge = document.createElement('span');
    badge.className = 'rounded-full bg-emerald-500/10 px-2 py-1 font-medium text-emerald-400';
    badge.textContent = 'Completed';
    meta.appendChild(badge);
  }

  const arrow = document.createElement('span');
  arrow.className =
    'hidden text-slate-600 opacity-0 transition duration-300 group-hover:translate-x-0.5 group-hover:text-emerald-400 group-hover:opacity-100 sm:inline';
  arrow.setAttribute('aria-hidden', 'true');
  arrow.textContent = '→';
  meta.appendChild(arrow);

  row.append(leftGroup, meta);
  row.addEventListener('click', () => navigate(`/lesson/${lesson.slug}`));
  return row;
}

function createCapstoneCard(capstone) {
  const card = document.createElement('section');
  card.className =
    'flex flex-col gap-4 rounded-2xl border border-emerald-500/20 bg-gradient-to-br from-emerald-500/10 to-violet-500/10 p-6 shadow-lg shadow-black/10';
  card.setAttribute('aria-label', 'Capstone project');

  const eyebrow = document.createElement('span');
  eyebrow.className = 'flex items-center gap-1.5 text-xs font-semibold uppercase tracking-wide text-emerald-400';
  eyebrow.textContent = '🏁 Capstone Project';

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

  const checklist = createChecklist(capstone.checklistItems);

  card.append(eyebrow, title, description, requirementsHeading, requirements, checklistHeading, checklist);
  return card;
}
