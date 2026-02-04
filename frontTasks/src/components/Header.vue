<script setup lang="ts">
import { ref } from 'vue'
import { PhCheck } from '@phosphor-icons/vue'
import { ToggleSwitch } from 'primevue'

const kanbanMode = ref(false)

const emit = defineEmits<{
    (e: 'mode-changed', value: boolean): void
}>()

const toggleKanbanMode = (value: boolean) => {
    kanbanMode.value = value
    emit('mode-changed', value)
}
</script>

<template>  
  <header class="flex fixed top-0 left-0 right-0 w-full h-16 bg-white shadow-md  justify-between items-center pl-5">
    <div class="flex items-center gap-2">
     <h1 class="text-2xl font-bold">TaksNoter</h1>  
     <div class="flex items-center gap-2 z">
      <div class="bg-gradient-to-tr from-blue-200 to-blue-600 text-white px-2 py-1 rounded-md">
        <PhCheck :size="24" />
      </div>
    </div>
    </div>
    <div class="flex items-center gap-2 pr-2">
        <p class="text-xs">Kanban Mode</p>
        <ToggleSwitch v-model="kanbanMode" @update:modelValue="toggleKanbanMode" color="blue"/>
    </div>
  </header>
</template>