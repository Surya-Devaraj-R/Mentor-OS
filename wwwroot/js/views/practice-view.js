import { getExercises, getExercise, submitExerciseAttempt } from '../api/exercises.js';
import { createBreadcrumb } from '../components/breadcrumb.js';
import { renderMiniMarkdown } from '../components/content-blocks/mini-markdown.js';
import { renderCodeSnippet } from '../components/content-blocks/code-snippet.js';
import { createLoadingMessage, createErrorMessage } from '../components/status-message.js';
import { navigate, refresh } from '../router.js';

const STATUS_LABELS = { Solved: '✓ Solved', Attempted: '◐ Attempted', NeedsReview: '⚠ Needs Review' };

// Route: /practice
export async function renderPracticeListView(params, query, root) {
  document.title = 'Practice · Mentor OS';
  root.replaceChildren(createLoadingMessage('Loading exercises…'));
  const controller = new AbortController();

  try {
    const exercises = await getExercises({}, { signal: controller.signal });
    root.replaceChildren(buildListView(exercises));
  } catch (error) {
    if (error.name === 'AbortError') return;
    root.replaceChildren(createErrorMessage(error, 'Something went wrong loading exercises.'));
  }

  return () => controller.abort();
}

function buildListView(exercises) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-6';

  const heading = document.createElement('h1');
  heading.className = 'text-2xl font-semibold tracking-tight text-slate-50';
  heading.textContent = 'Practice';

  const grid = document.createElement('div');
  grid.className = 'grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3';
  exercises.forEach((exercise) => grid.appendChild(createExerciseCard(exercise)));

  container.append(heading, grid);
  return container;
}

function createExerciseCard(exercise) {
  const card = document.createElement('button');
  card.type = 'button';
  card.className =
    'flex flex-col gap-2 rounded-xl border border-white/5 bg-slate-800 p-5 text-left shadow-lg shadow-black/20 transition duration-300 hover:-translate-y-1 hover:border-emerald-500/30 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';

  const title = document.createElement('h3');
  title.className = 'text-base font-semibold text-slate-50';
  title.textContent = exercise.title;

  const meta = document.createElement('div');
  meta.className = 'flex flex-wrap gap-2 text-xs text-slate-400';

  const difficulty = document.createElement('span');
  difficulty.className = 'rounded-full bg-slate-900 px-2 py-1';
  difficulty.textContent = exercise.difficultyLevel;
  meta.appendChild(difficulty);

  if (exercise.language) {
    const language = document.createElement('span');
    language.className = 'rounded-full bg-slate-900 px-2 py-1 uppercase';
    language.textContent = exercise.language;
    meta.appendChild(language);
  }

  if (exercise.latestStatus) {
    const status = document.createElement('span');
    status.className = 'rounded-full bg-emerald-500/10 px-2 py-1 text-emerald-400';
    status.textContent = STATUS_LABELS[exercise.latestStatus] ?? exercise.latestStatus;
    meta.appendChild(status);
  }

  card.append(title, meta);
  card.addEventListener('click', () => navigate(`/practice/${exercise.slug}`));
  return card;
}

// Route: /practice/:exerciseSlug
export async function renderPracticeDetailView(params, query, root) {
  root.replaceChildren(createLoadingMessage('Loading exercise…'));
  const controller = new AbortController();

  try {
    const exercise = await getExercise(params.exerciseSlug, { signal: controller.signal });
    document.title = `${exercise.title} · Mentor OS`;
    root.replaceChildren(buildDetailView(exercise));
  } catch (error) {
    if (error.name === 'AbortError') return;
    document.title = 'Exercise Not Found · Mentor OS';
    root.replaceChildren(createErrorMessage(error, 'That exercise could not be found.'));
  }

  return () => controller.abort();
}

function buildDetailView(exercise) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-6';

  const header = document.createElement('header');
  header.className = 'flex flex-col gap-3';
  header.append(
    createBreadcrumb([{ label: 'Practice', hash: '#/practice' }, { label: exercise.title }]),
  );

  const heading = document.createElement('h1');
  heading.className = 'text-2xl font-semibold tracking-tight text-slate-50';
  heading.textContent = exercise.title;
  header.appendChild(heading);

  const promptSection = document.createElement('section');
  promptSection.className = 'rounded-xl border border-white/5 bg-slate-800 p-5';
  promptSection.appendChild(renderMiniMarkdown(exercise.prompt));

  const attemptForm = createAttemptForm(exercise);
  const solutionsSection = createSolutionsSection(exercise.solutions);
  const historySection = createHistorySection(exercise.submissions);

  container.append(header, promptSection, attemptForm, solutionsSection, historySection);
  return container;
}

function createAttemptForm(exercise) {
  const form = document.createElement('form');
  form.className = 'flex flex-col gap-3 rounded-xl border border-white/5 bg-slate-800 p-5';

  const label = document.createElement('h2');
  label.className = 'text-xs font-semibold uppercase tracking-wide text-emerald-400';
  label.textContent = 'Your Attempt';

  const codeInput = document.createElement('textarea');
  codeInput.rows = 10;
  codeInput.required = true;
  codeInput.value = exercise.starterCode ?? '';
  codeInput.spellcheck = false;
  codeInput.className =
    'w-full rounded-lg border border-white/10 bg-slate-900 p-3 font-mono text-xs text-slate-100 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  codeInput.setAttribute('aria-label', 'Your attempt code');

  const notesInput = document.createElement('textarea');
  notesInput.rows = 2;
  notesInput.placeholder = 'Notes on your approach (optional)';
  notesInput.className =
    'w-full rounded-lg border border-white/10 bg-slate-900 px-3 py-2 text-sm text-slate-50 placeholder:text-slate-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  notesInput.setAttribute('aria-label', 'Notes on your approach');

  const actionsRow = document.createElement('div');
  actionsRow.className = 'flex flex-wrap items-center justify-between gap-3';

  const assessmentGroup = document.createElement('div');
  assessmentGroup.setAttribute('role', 'radiogroup');
  assessmentGroup.setAttribute('aria-label', 'Self-assessment');
  assessmentGroup.className = 'flex gap-2';

  let selectedAssessment = 'Attempted';
  const assessmentButtons = ['Attempted', 'Solved', 'NeedsReview'].map((value) => {
    const button = document.createElement('button');
    button.type = 'button';
    button.textContent = STATUS_LABELS[value] ?? value;
    button.dataset.value = value;
    button.setAttribute('role', 'radio');
    button.className = getAssessmentButtonClass(value === selectedAssessment);
    button.setAttribute('aria-checked', String(value === selectedAssessment));
    button.addEventListener('click', () => {
      selectedAssessment = value;
      assessmentButtons.forEach((btn) => {
        const isSelected = btn.dataset.value === selectedAssessment;
        btn.className = getAssessmentButtonClass(isSelected);
        btn.setAttribute('aria-checked', String(isSelected));
      });
    });
    assessmentGroup.appendChild(button);
    return button;
  });

  const submitButton = document.createElement('button');
  submitButton.type = 'submit';
  submitButton.className =
    'rounded-lg border border-emerald-500/40 bg-emerald-500/10 px-4 py-2 text-sm font-medium text-emerald-400 transition duration-300 hover:bg-emerald-500/20 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  submitButton.textContent = 'Log Attempt';

  actionsRow.append(assessmentGroup, submitButton);
  form.append(label, codeInput, notesInput, actionsRow);

  form.addEventListener('submit', async (event) => {
    event.preventDefault();
    const submittedCode = codeInput.value.trim();
    if (!submittedCode) return;

    try {
      await submitExerciseAttempt(exercise.id, {
        submittedCode,
        notes: notesInput.value.trim() || null,
        selfAssessment: selectedAssessment,
      });
      await refresh();
    } catch (error) {
      console.error('Failed to log attempt:', error);
    }
  });

  return form;
}

function getAssessmentButtonClass(isSelected) {
  const base =
    'rounded-lg border px-3 py-1.5 text-xs font-medium transition duration-300 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  return isSelected
    ? `${base} border-emerald-500/40 bg-emerald-500/10 text-emerald-400`
    : `${base} border-white/10 text-slate-400 hover:text-slate-50`;
}

function createSolutionsSection(solutions) {
  const section = document.createElement('section');
  section.className = 'flex flex-col gap-3';

  const heading = document.createElement('h2');
  heading.className = 'text-lg font-semibold tracking-tight text-slate-50';
  heading.textContent = 'Solutions';
  section.appendChild(heading);

  const details = document.createElement('details');
  details.className = 'rounded-xl border border-white/5 bg-slate-800 p-4';
  const summary = document.createElement('summary');
  summary.className = 'cursor-pointer text-sm font-medium text-emerald-400';
  summary.textContent = `Reveal ${solutions.length} solution${solutions.length === 1 ? '' : 's'}`;
  details.appendChild(summary);

  const solutionsList = document.createElement('div');
  solutionsList.className = 'mt-4 flex flex-col gap-4';
  solutions.forEach((solution) => solutionsList.appendChild(createSolutionCard(solution)));
  details.appendChild(solutionsList);

  section.appendChild(details);
  return section;
}

function createSolutionCard(solution) {
  const card = document.createElement('div');
  card.className = 'flex flex-col gap-2 rounded-lg border border-white/5 bg-slate-900/50 p-4';

  const titleRow = document.createElement('div');
  titleRow.className = 'flex flex-wrap items-center justify-between gap-2';

  const title = document.createElement('h3');
  title.className = 'text-sm font-semibold text-slate-50';
  title.textContent = solution.approachTitle;

  const complexity = document.createElement('span');
  complexity.className = 'text-xs text-slate-500';
  complexity.textContent = [solution.timeComplexity, solution.spaceComplexity].filter(Boolean).join(' · ');

  titleRow.append(title, complexity);

  const explanation = renderMiniMarkdown(solution.explanation);
  const code = renderCodeSnippet({ body: solution.solutionCode, language: solution.language });

  card.append(titleRow, explanation, code);
  return card;
}

function createHistorySection(submissions) {
  const section = document.createElement('section');
  section.className = 'flex flex-col gap-3';

  const heading = document.createElement('h2');
  heading.className = 'text-lg font-semibold tracking-tight text-slate-50';
  heading.textContent = 'Your Attempt History';
  section.appendChild(heading);

  if (submissions.length === 0) {
    const empty = document.createElement('p');
    empty.className = 'text-sm text-slate-400';
    empty.textContent = 'No attempts logged yet.';
    section.appendChild(empty);
    return section;
  }

  const list = document.createElement('div');
  list.className = 'flex flex-col gap-2';
  submissions.forEach((submission) => {
    const row = document.createElement('div');
    row.className = 'flex items-center justify-between gap-3 rounded-lg border border-white/5 bg-slate-800 px-4 py-2 text-xs text-slate-400';

    const label = document.createElement('span');
    label.textContent = `Attempt #${submission.attemptNumber} · ${new Date(submission.submittedUtc).toLocaleString()}`;

    const status = document.createElement('span');
    status.className = 'font-medium text-emerald-400';
    status.textContent = STATUS_LABELS[submission.selfAssessment] ?? submission.selfAssessment;

    row.append(label, status);
    list.appendChild(row);
  });

  section.appendChild(list);
  return section;
}
