import { getInterviewQuestions, setInterviewQuestionCompleted, getCompanies } from '../api/interviewPrep.js';
import { getProgressSummary } from '../api/progress.js';
import { renderMiniMarkdown } from '../components/content-blocks/mini-markdown.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';
import { setState } from '../state.js';
import { navigate } from '../router.js';

const GROUP_LABELS = {
  Behavioral: 'Behavioral',
  Technical: 'Technical',
  SystemDesign: 'System Design',
  MockInterviewChecklist: 'Mock Interview Checklists',
};

// Route: /interview-prep (optionally ?company=slug)
export async function renderInterviewPrepView(params, query, root) {
  document.title = 'Interview Prep · Mentor OS';
  root.replaceChildren(createLoadingMessage('Loading interview prep…'));
  const controller = new AbortController();
  const activeCompany = query.get('company') || null;

  try {
    const [questions, companies] = await Promise.all([
      getInterviewQuestions({ company: activeCompany }, { signal: controller.signal }),
      getCompanies({ signal: controller.signal }),
    ]);
    root.replaceChildren(buildView(questions, companies, activeCompany));
  } catch (error) {
    if (error.name === 'AbortError') return;
    root.replaceChildren(createErrorMessage(error, 'Something went wrong loading interview prep.'));
  }

  return () => controller.abort();
}

function buildView(questions, companies, activeCompany) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-8';

  const heading = document.createElement('h1');
  heading.className = 'text-2xl font-semibold tracking-tight text-slate-50';
  heading.textContent = 'Interview Prep';
  container.appendChild(heading);

  const companyFilterRow = document.createElement('div');
  companyFilterRow.className = 'flex flex-wrap gap-2';
  companyFilterRow.appendChild(createCompanyChip('All Companies', null, activeCompany));
  companies.forEach((company) => companyFilterRow.appendChild(createCompanyChip(company.name, company.slug, activeCompany)));
  container.appendChild(companyFilterRow);

  const activeCompanyData = companies.find((c) => c.slug === activeCompany);
  if (activeCompanyData?.overviewBody) {
    const overview = document.createElement('div');
    overview.className = 'rounded-xl border border-emerald-500/20 bg-emerald-500/5 p-5';
    overview.appendChild(renderMiniMarkdown(activeCompanyData.overviewBody));
    container.appendChild(overview);
  }

  for (const [type, label] of Object.entries(GROUP_LABELS)) {
    const groupQuestions = questions.filter((q) => q.questionType === type);
    if (groupQuestions.length === 0) continue;

    const section = document.createElement('section');
    section.className = 'flex flex-col gap-3';

    const groupHeading = document.createElement('h2');
    groupHeading.className = 'text-lg font-semibold tracking-tight text-slate-50';
    groupHeading.textContent = label;

    const list = document.createElement('div');
    list.className = 'flex flex-col gap-3';
    groupQuestions.forEach((question) => list.appendChild(createQuestionCard(question)));

    section.append(groupHeading, list);
    container.appendChild(section);
  }

  if (questions.length === 0) {
    const empty = document.createElement('p');
    empty.className = 'text-sm text-slate-400';
    empty.textContent = 'No questions match this filter yet.';
    container.appendChild(empty);
  }

  return container;
}

function createCompanyChip(label, slug, activeCompany) {
  const isActive = slug === activeCompany;
  const chip = document.createElement('button');
  chip.type = 'button';
  chip.className = isActive
    ? 'rounded-full border border-emerald-500/40 bg-emerald-500/10 px-3 py-1 text-xs font-medium text-emerald-400'
    : 'rounded-full border border-white/10 px-3 py-1 text-xs text-slate-400 transition duration-300 hover:text-slate-50';
  chip.textContent = label;
  chip.addEventListener('click', () => navigate(slug ? `/interview-prep?company=${encodeURIComponent(slug)}` : '/interview-prep'));
  return chip;
}

function createQuestionCard(question) {
  const details = document.createElement('details');
  details.className = 'rounded-xl border border-white/5 bg-slate-800 p-5 shadow-lg shadow-black/20';

  const summary = document.createElement('summary');
  summary.className = 'flex cursor-pointer items-center justify-between gap-3 text-sm font-semibold text-slate-50';

  const titleCol = document.createElement('div');
  titleCol.className = 'flex flex-col gap-1';

  const title = document.createElement('span');
  title.textContent = question.title;
  titleCol.appendChild(title);

  if (question.companies.length > 0) {
    const companyRow = document.createElement('div');
    companyRow.className = 'flex flex-wrap gap-1';
    question.companies.forEach((company) => {
      const chip = document.createElement('span');
      chip.className = 'rounded-full bg-slate-900 px-2 py-0.5 text-xs font-normal text-slate-400';
      chip.textContent = company;
      companyRow.appendChild(chip);
    });
    titleCol.appendChild(companyRow);
  }

  const checkboxLabel = document.createElement('label');
  checkboxLabel.className = 'flex shrink-0 items-center gap-2 text-xs font-normal text-slate-400';
  checkboxLabel.addEventListener('click', (event) => event.stopPropagation());

  const checkbox = document.createElement('input');
  checkbox.type = 'checkbox';
  checkbox.checked = question.isCompleted;
  checkbox.className = 'h-4 w-4 rounded accent-emerald-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  checkbox.setAttribute('aria-label', `Mark "${question.title}" as reviewed`);

  checkbox.addEventListener('change', async () => {
    const wasChecked = !checkbox.checked;
    try {
      await setInterviewQuestionCompleted(question.id, checkbox.checked);
      setState('progress', await getProgressSummary());
    } catch (error) {
      checkbox.checked = wasChecked;
      console.error('Failed to update interview question completion:', error);
    }
  });

  checkboxLabel.append(checkbox, document.createTextNode('Reviewed'));
  summary.append(titleCol, checkboxLabel);

  const body = document.createElement('div');
  body.className = 'mt-4 flex flex-col gap-4';
  body.appendChild(renderMiniMarkdown(question.promptText));

  if (question.suggestedApproach) {
    body.appendChild(createSubsection('Suggested Approach', question.suggestedApproach));
  }
  if (question.sampleAnswer) {
    body.appendChild(createSubsection('Sample Answer', question.sampleAnswer));
  }

  details.append(summary, body);
  return details;
}

function createSubsection(label, text) {
  const wrapper = document.createElement('div');
  wrapper.className = 'flex flex-col gap-2';

  const heading = document.createElement('h3');
  heading.className = 'text-xs font-semibold uppercase tracking-wide text-emerald-400';
  heading.textContent = label;

  wrapper.append(heading, renderMiniMarkdown(text));
  return wrapper;
}
