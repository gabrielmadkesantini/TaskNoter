import { createApp } from 'vue'
import { createPinia } from 'pinia'

import './assets/main.css'
import 'primeicons/primeicons.css'
import PrimeVue from 'primevue/config'
import Aura from '@primeuix/themes/aura';
import App from './App.vue'
import router from './router'

// Variável para controlar o tema (pode ser 'light' ou 'dark')
// Esta variável define o tema independente do sistema operacional
// Altere para 'dark' se quiser usar o tema escuro
const themeMode = 'light' as 'light' | 'dark'

const isDarkMode = themeMode === 'dark'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(PrimeVue, {
  theme: {
    preset: Aura,
    options: {
      // Desabilita a detecção automática do tema do sistema
      // Usa um seletor customizado baseado na variável
      darkModeSelector: isDarkMode ? '.dark-mode' : false,
    },
  },
})

// Força o tema baseado na variável, removendo qualquer classe de tema do sistema
if (isDarkMode) {
  document.documentElement.classList.add('dark-mode')
  document.documentElement.classList.remove('light-mode')
} else {
  document.documentElement.classList.remove('dark-mode')
  document.documentElement.classList.add('light-mode')
}

app.mount('#app')
