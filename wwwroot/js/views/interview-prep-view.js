import { getInterviewQuestions, setInterviewQuestionCompleted } from '../api/interviewPrep.js';
import { getProgressSummary } from '../api/progress.js';
import { renderMiniMarkdown } from '../components/content-blocks/mini-markdown.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';
import { setState } from '../state.js';

const GROUP_LABELS = {
  Behavioral: 'Behavioral',
  SystemDesign: 'System Design',
  MockInterviewChecklist: 'Mock Interview Checklists',
};

// Route: /interview-prep
export async function renderInterviewPrepView(params, query, root) {
  document.title = 'Interview Prep · Mentor OS';
  root.replaceChildren(createLoadingMessage('Loading interview prep…'));
  const controller = new AbortController();

  try {
    const questions = await getInterviewQuestions(null, { signal: controller.signal });
    root.replaceChildren(buildView(questions));
  } catch (error) {
    if (error.name === 'AbortError') return;
    root.replaceChildren(createErrorMessage(error, 'Something went wrong loading interview prep.'));
  }

  return () => controller.abort();
}

function buildView(questions) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-8';

  const heading = document.createElement('h1');
  heading.className = 'text-2xl font-semibold tracking-tight text-slate-50';
  heading.textContent = 'Interview Prep';
  container.appendChild(heading);

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

  return container;
}

function createQuestionCard(question) {
  const details = document.createElement('details');
  details.className = 'rounded-xl border border-white/5 bg-slate-800 p-5 shadow-lg shadow-black/20';

  const summary = document.createElement('summary');
  summary.className = 'flex cursor-pointer items-center justify-between gap-3 text-sm font-semibold text-slate-50';

  const title = document.createElement('span');
  title.textContent = question.title;

  const checkboxLabel = document.createElement('label');
  checkboxLabel.className = 'flex items-center gap-2 text-xs font-normal text-slate-400';
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
  summary.append(title, checkboxLabel);

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
