// Ungraded, client-side only self-check — no attempt persistence. Pick an
// option, get instant correct/incorrect feedback plus the explanation.
export function createQuiz(questions) {
  const container = document.createElement('div');
  container.className = 'flex flex-col gap-4';

  questions.forEach((question, index) => container.appendChild(createQuestion(question, index)));

  return container;
}

function createQuestion(question, index) {
  const card = document.createElement('fieldset');
  card.className = 'flex flex-col gap-3 rounded-lg border border-white/5 bg-slate-900/50 p-4';

  const legend = document.createElement('legend');
  legend.className = 'px-1 text-sm font-medium text-slate-50';
  legend.textContent = `${index + 1}. ${question.questionText}`;
  card.appendChild(legend);

  const groupName = `quiz-question-${question.id}`;
  const optionRows = question.options.map((option) => createOption(option, groupName));
  optionRows.forEach(({ row }) => card.appendChild(row));

  const feedback = document.createElement('div');
  feedback.className = 'hidden text-sm';
  feedback.setAttribute('role', 'status');

  const checkButton = document.createElement('button');
  checkButton.type = 'button';
  checkButton.className =
    'self-start rounded-lg border border-emerald-500/40 bg-emerald-500/10 px-3 py-1.5 text-xs font-medium text-emerald-400 transition duration-300 hover:bg-emerald-500/20 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';
  checkButton.textContent = 'Check Answer';

  checkButton.addEventListener('click', () => {
    const selected = optionRows.find(({ input }) => input.checked);
    if (!selected) {
      feedback.className = 'text-sm text-amber-400';
      feedback.textContent = 'Pick an option first.';
      return;
    }

    optionRows.forEach(({ input, option, row }) => {
      row.classList.remove('border-emerald-500/40', 'border-red-500/40');
      if (option.isCorrect) row.classList.add('border-emerald-500/40');
      else if (input === selected.input) row.classList.add('border-red-500/40');
    });

    feedback.className = selected.option.isCorrect ? 'text-sm text-emerald-400' : 'text-sm text-red-400';
    feedback.textContent = selected.option.isCorrect
      ? `Correct! ${question.explanation}`
      : `Not quite. ${question.explanation}`;
  });

  card.append(checkButton, feedback);
  return card;
}

function createOption(option, groupName) {
  const row = document.createElement('label');
  row.className =
    'flex cursor-pointer items-center gap-2 rounded-lg border border-white/10 px-3 py-2 text-sm text-slate-300 transition duration-300 hover:border-white/20';

  const input = document.createElement('input');
  input.type = 'radio';
  input.name = groupName;
  input.className = 'h-4 w-4 accent-emerald-500 focus:outline-none focus:ring-2 focus:ring-emerald-500/50';

  row.append(input, document.createTextNode(option.text));
  return { row, input, option };
}
