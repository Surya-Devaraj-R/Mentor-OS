import { getProgressSummary } from '../api/progress.js';
import { getTopics } from '../api/topics.js';
import { createProgressBar } from '../components/progress-bar.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';
import { setState } from '../state.js';
import { navigate } from '../router.js';
import { getTopicIcon } from '../utils/icons.js';

export async function renderDashboardView(params, query, root) {
  document.title = 'Dashboard · Mentor OS';
  root.replaceChildren(createLoadingMessage('Loading dashboard…'));

  const controller = new AbortController();

  try {
    const [summary, topics] = await Promise.all([
      getProgressSummary({ signal: controller.signal }),
      getTopics({ signal: controller.signal }),
    ]);
    setState('progress', summary);
    root.replaceChildren(buildView(summary, topics));
  } catch (error) {
    if (error.name === 'AbortError') return;
    root.replaceChildren(createErrorMessage(error, 'Something went wrong loading the dashboard.'));
  }

  return () => controller.abort();
}

function buildView(summary, topics) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-10 animate-fade-in-up';

  const hero = document.createElement('header');
  hero.className = 'flex flex-col gap-3 text-center';
  hero.innerHTML = `
    <span class="mx-auto flex items-center gap-1.5 rounded-full border border-emerald-500/20 bg-gradient-to-r from-emerald-500/15 to-violet-500/10 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-emerald-300">
      ✨ Your learning OS
    </span>
    <h1 class="text-gradient text-3xl font-extrabold tracking-tight drop-shadow-[0_0_25px_rgba(139,92,246,0.25)] sm:text-4xl">Mentor OS</h1>
    <p class="mx-auto max-w-xl text-sm text-slate-400 sm:text-base">
      Your personal learning operating system for C#, .NET, DSA, System Design, SQL, Cloud, and interview prep.
    </p>
  `;

  const topicsHeading = document.createElement('h2');
  topicsHeading.className = 'text-lg font-semibold tracking-tight text-slate-50';
  topicsHeading.textContent = 'Learning Roadmap';

  const topicGrid = document.createElement('div');
  topicGrid.className = 'grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3';
  topics.forEach((topic) => topicGrid.appendChild(createTopicCard(topic)));

  container.append(hero, createProgressBar(summary), topicsHeading, topicGrid);
  return container;
}

function createTopicCard(topic) {
  const card = document.createElement('button');
  card.type = 'button';
  card.className =
    'group flex flex-col gap-3 rounded-2xl border border-white/5 bg-slate-800/60 p-5 text-left shadow-lg shadow-black/20 ring-1 ring-white/5 backdrop-blur transition duration-300 hover:-translate-y-1 hover:border-emerald-500/30 hover:shadow-[0_8px_30px_rgba(16,185,129,0.15),0_10px_40px_rgba(139,92,246,0.12)] focus:outline-none focus:ring-2 focus:ring-emerald-500/50';

  const topRow = document.createElement('div');
  topRow.className = 'flex items-start justify-between gap-3';

  const iconBadge = document.createElement('span');
  iconBadge.className =
    'flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-gradient-to-br from-emerald-500/15 via-slate-800 to-violet-500/15 text-lg ring-1 ring-white/10';
  iconBadge.setAttribute('aria-hidden', 'true');
  iconBadge.textContent = getTopicIcon(topic);

  const arrow = document.createElement('span');
  arrow.className =
    'translate-x-0 text-slate-600 opacity-0 transition duration-300 group-hover:translate-x-0.5 group-hover:text-emerald-400 group-hover:opacity-100';
  arrow.setAttribute('aria-hidden', 'true');
  arrow.textContent = '→';

  topRow.append(iconBadge, arrow);

  const title = document.createElement('h3');
  title.className = 'text-base font-semibold text-slate-50';
  title.textContent = topic.title;

  const description = document.createElement('p');
  description.className = 'text-sm text-slate-400';
  description.textContent = topic.description;

  card.append(topRow, title, description);
  card.addEventListener('click', () => navigate(`/roadmap/${topic.slug}`));
  return card;
}
