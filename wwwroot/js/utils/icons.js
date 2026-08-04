// Per-topic emoji identity, with a generic fallback keyed off the backend's
// coarser IconKey field (course/system/practice) for any topic not listed here.
const TOPIC_ICONS = {
  'csharp-basics': '🌱',
  csharp: '🟣',
  dotnet: '🔷',
  dsa: '🧩',
  'system-design': '🏗️',
  sql: '🗄️',
  cloud: '☁️',
  git: '🌿',
  devops: '⚙️',
  architecture: '🏛️',
  'soft-skills': '🤝',
  'ai-integration': '🤖',
  frontend: '🎨',
};

const ICON_KEY_FALLBACKS = {
  course: '📘',
  system: '🖥️',
  practice: '🧠',
};

export function getTopicIcon(topic) {
  return TOPIC_ICONS[topic.slug] ?? ICON_KEY_FALLBACKS[topic.iconKey] ?? '📘';
}
