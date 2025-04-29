import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '@/views/LoginView.vue';
import RegisterView from '@/views/RegisterView.vue';
import ScheduleView from '@/views/ScheduleView.vue';
import IncidentView from '@/views/IncidentView.vue';
import ProfileView from '@/views/ProfileView.vue';
import TeamView from '@/views/TeamView.vue';

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: LoginView },
  { path: '/register', component: RegisterView },
  { path: '/schedule', component: ScheduleView },
  { path: '/incident', component: IncidentView },
  { path: '/profile', component: ProfileView },
  { path: '/team', component: TeamView },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
