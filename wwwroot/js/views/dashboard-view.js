import { getProgressSummary } from '../api/progress.js';
import { getTopics } from '../api/topics.js';
import { createProgressBar } from '../components/progress-bar.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';
import { setState } from '../state.js';
import { navigate } from '../router.js';

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
  container.className = 'flex flex-col gap-10';

  const hero = document.createElement('header');
  hero.className = 'flex flex-col gap-3 text-center';
  hero.innerHTML = `
    <h1 class="text-3xl font-semibold tracking-tight text-slate-50 sm:text-4xl">Mentor OS</h1>
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
    'flex flex-col gap-2 rounded-xl border border-white/5 bg-slate-800 p-5 text-left shadow-lg shadow-black/20 transition duration-300 hover:-translate-y-1 hover:border-emerald-500/30 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';

  const title = document.createElement('h3');
  title.className = 'text-base font-semibold text-slate-50';
  title.textContent = topic.title;

  const description = document.createElement('p');
  description.className = 'text-sm text-slate-400';
  description.textContent = topic.description;

  card.append(title, description);
  card.addEventListener('click', () => navigate(`/roadmap/${topic.slug}`));
  return card;
}
