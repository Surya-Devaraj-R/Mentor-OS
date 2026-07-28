import { getProjectForTopic } from '../api/projects.js';
import { getTopic } from '../api/topics.js';
import { createBreadcrumb } from '../components/breadcrumb.js';
import { renderMiniMarkdown } from '../components/content-blocks/mini-markdown.js';
import { renderStructuredSteps } from '../components/content-blocks/structured-steps.js';
import { renderAsciiArt } from '../components/content-blocks/ascii-art.js';
import { createChecklist } from '../components/checklist.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';

// Route: /projects/:topicSlug
export async function renderProjectView(params, query, root) {
  root.replaceChildren(createLoadingMessage('Loading project…'));
  const controller = new AbortController();

  try {
    const [topic, project] = await Promise.all([
      getTopic(params.topicSlug, { signal: controller.signal }),
      getProjectForTopic(params.topicSlug, { signal: controller.signal }),
    ]);
    document.title = `${project.title} · Mentor OS`;
    root.replaceChildren(buildView(topic, project));
  } catch (error) {
    if (error.name === 'AbortError') return;
    document.title = 'Project Not Found · Mentor OS';
    root.replaceChildren(createErrorMessage(error, 'That project could not be found.'));
  }

  return () => controller.abort();
}

function buildView(topic, project) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-8';

  const header = document.createElement('header');
  header.className = 'flex flex-col gap-3';
  header.append(
    createBreadcrumb([
      { label: 'Dashboard', hash: '#/' },
      { label: topic.title, hash: `#/roadmap/${topic.slug}` },
      { label: project.title },
    ]),
  );

  const eyebrow = document.createElement('span');
  eyebrow.className = 'text-xs font-semibold uppercase tracking-wide text-emerald-400';
  eyebrow.textContent = 'Production Project';

  const heading = document.createElement('h1');
  heading.className = 'text-2xl font-semibold tracking-tight text-slate-50';
  heading.textContent = project.title;

  header.append(eyebrow, heading);

  const descriptionSection = document.createElement('section');
  descriptionSection.className = 'rounded-xl border border-white/5 bg-slate-800 p-5';
  descriptionSection.appendChild(renderMiniMarkdown(project.description));

  const diagramSection = buildSection('Architecture', renderDiagram(project));
  const milestonesSection = buildSection('Milestones', createMilestonesList(project.milestones));
  const checklistSection = buildSection('Implementation Checklist', createChecklist(project.checklist));
  const portfolioSection = buildSection('Portfolio Guidance', renderMiniMarkdown(project.portfolioGuidance));

  container.append(header, descriptionSection, diagramSection, milestonesSection, checklistSection, portfolioSection);
  return container;
}

function renderDiagram(project) {
  const wrapper = document.createElement('div');
  wrapper.className = 'rounded-xl border border-white/5 bg-slate-800 p-5';
  wrapper.appendChild(
    project.architectureDiagramFormat === 'AsciiArt'
      ? renderAsciiArt(project.architectureDiagramBody)
      : renderStructuredSteps(project.architectureDiagramBody),
  );
  return wrapper;
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

function createMilestonesList(milestones) {
  const list = document.createElement('ol');
  list.className = 'flex flex-col gap-3';

  milestones.forEach((milestone, index) => {
    const item = document.createElement('li');
    item.className = 'flex gap-3 rounded-lg border border-white/5 bg-slate-800 p-4';

    const number = document.createElement('span');
    number.className = 'flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-emerald-500/10 text-xs font-semibold text-emerald-400';
    number.textContent = String(index + 1);

    const textCol = document.createElement('div');
    textCol.className = 'flex flex-col gap-0.5';

    const title = document.createElement('h3');
    title.className = 'text-sm font-semibold text-slate-50';
    title.textContent = milestone.title;

    const description = document.createElement('p');
    description.className = 'text-sm text-slate-400';
    description.textContent = milestone.description;

    textCol.append(title, description);
    item.append(number, textCol);
    list.appendChild(item);
  });

  return list;
}
