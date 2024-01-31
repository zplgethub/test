import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue'
import MainView from '../views/MainView.vue'
import StudentView from '../views/StudentView.vue'
import ClassesView from '../views/ClassesView.vue'
import CourseView from '../views/CourseView.vue'
import ScoreView from '../views/ScoreView.vue'
import PersonalView from '../views/PersonalView.vue'
import MenuView from '../views/MenuView.vue'
import RoleView from '../views/RoleView.vue'
import UserView from '../views/UserView.vue'

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'home1',
    component: HomeView
  },
  {
    path: '/home',
    name: 'home',
    component: HomeView,
	children:[
		{
			path:'/main',
			name:'main',
			component:MainView
		},
		{
			path:'/student',
			name:'student',
			component:StudentView,
		},
		{
			path:'/classes',
			name:'classes',
			component:ClassesView
		},
		{
			path:'/course',
			name:'course',
			component:CourseView
		},
		{
			path:'/score',
			name:'score',
			component:ScoreView
		},
		{
			path:'/personal',
			name:'personal',
			component:PersonalView
		},
		{
			path:'/menu',
			name:'menu',
			component:MenuView
		},
		{
			path:'/user',
			name:'user',
			component:UserView
		},
		{
			path:'/role',
			name:'role',
			component:RoleView
		}
	]
  },
  {
	path:'/login',
	name:'login',
	component:LoginView
  },
  
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
