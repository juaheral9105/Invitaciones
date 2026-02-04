import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: () => import('../views/Home.vue')
  },
  {
    path: '/editor',
    name: 'Editor',
    component: () => import('../views/Editor/EditorViewNew.vue')
  },
  {
    path: '/editor/:id',
    name: 'EditorEdit',
    component: () => import('../views/Editor/EditorViewNew.vue')
  },
  {
    path: '/invitation/:id',
    name: 'InvitationView',
    component: () => import('../views/Viewer/InvitationView.vue')
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
