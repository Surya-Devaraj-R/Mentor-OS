// Renders a JSON array of {label, note?} as a horizontal flexbox chain of
// boxes with arrow connectors — for linear flows (request pipelines,
// pipelines, call chains). Real <ol>/<li> semantics, not bare divs, so
// screen readers get an actual list instead of a wall of unordered boxes.
export function renderStructuredSteps(bodyJson) {
  const steps = JSON.parse(bodyJson);

  const list = document.createElement('ol');
  list.className = 'flex flex-wrap items-center gap-x-2 gap-y-3';
  list.setAttribute('aria-label', 'Steps');

  steps.forEach((step, index) => {
    const item = document.createElement('li');
    item.className = 'flex items-center gap-2';

    const box = document.createElement('div');
    box.className =
      'flex flex-col gap-0.5 rounded-lg border border-emerald-500/20 bg-emerald-500/5 px-3 py-2 text-xs';

    const label = document.createElement('div');
    label.className = 'font-semibold text-slate-50';
    label.textContent = step.label;
    box.appendChild(label);

    if (step.note) {
      const note = document.createElement('div');
      note.className = 'text-slate-400';
      note.textContent = step.note;
      box.appendChild(note);
    }

    item.appendChild(box);

    if (index < steps.length - 1) {
      const arrow = document.createElement('span');
      arrow.className = 'text-emerald-500';
      arrow.setAttribute('aria-hidden', 'true');
      arrow.textContent = '→';
      item.appendChild(arrow);
    }

    list.appendChild(item);
  });

  return list;
}
