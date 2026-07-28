import { initRouter, registerRoute } from './router.js';
import { renderNavBar } from './components/nav-bar.js';
import { renderDashboardView } from './views/dashboard-view.js';
import { renderResourcesView } from './views/resources-view.js';
import { renderTopicView, renderModuleView } from './views/roadmap-view.js';
import { renderLessonView } from './views/lesson-view.js';
import { renderPlannerView } from './views/planner-view.js';
import { renderNotesView } from './views/notes-view.js';
import { renderPracticeListView, renderPracticeDetailView } from './views/practice-view.js';
import { renderInterviewPrepView } from './views/interview-prep-view.js';
import { renderProjectView } from './views/project-view.js';
import { renderSearchView } from './views/search-view.js';
import { renderNotFoundView } from './views/not-found-view.js';

renderNavBar(document.getElementById('nav-bar-mount'));

registerRoute('/', renderDashboardView);
registerRoute('/resources', renderResourcesView);
registerRoute('/roadmap/:topicSlug', renderTopicView);
registerRoute('/roadmap/:topicSlug/:moduleSlug', renderModuleView);
registerRoute('/lesson/:lessonSlug', renderLessonView);
registerRoute('/planner', renderPlannerView);
registerRoute('/notes', renderNotesView);
registerRoute('/practice', renderPracticeListView);
registerRoute('/practice/:exerciseSlug', renderPracticeDetailView);
registerRoute('/interview-prep', renderInterviewPrepView);
registerRoute('/projects/:topicSlug', renderProjectView);
registerRoute('/search', renderSearchView);
registerRoute('*', renderNotFoundView);

initRouter();
