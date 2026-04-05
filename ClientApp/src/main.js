import { createApp } from 'vue'
import App from './App.vue'
import './index.css'
import SignalR from './plugins/signal-r'
import Auth from './plugins/auth-service'
import router from './router'
import VueApexCharts from "./plugins/apex";
// import 'es6-promise/auto'
import store from './store'

const app = createApp(App)
    .use(VueApexCharts)
    .use(router)
    .use(store)
    .use(Auth)
    .use(SignalR, { connection: `${import.meta.env.VITE_API}/weighthub` })

app.mount('#app')