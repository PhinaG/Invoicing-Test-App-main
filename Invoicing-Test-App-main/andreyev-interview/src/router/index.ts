import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'

const routes: Array<RouteRecordRaw> = [
  {
    path: '/invoices',
    name: 'Invoices',
    component: () => import('../views/Invoices.vue')
  },
  {
    path: '/invoices/:id',
    name: 'Invoice',
    component: () => import('../views/Invoice.vue'),
    props: true
  },
  {
    path: '/Clients',
    name: 'Clients',
    component: () => import('../views/Clients.vue')
  },
  { path: '/', redirect: '/invoices' }, 
  
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
