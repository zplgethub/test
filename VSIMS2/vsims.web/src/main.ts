import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
//1. 引入Antdv组件和样式
import Antd from 'ant-design-vue';
// import 'ant-design-vue/dist/antd.css';
const app = createApp(App);
app.use(router).use(Antd).mount('#app')
