<script setup lang="ts">
import { ref } from 'vue'
import Header from '../components/Header.vue'
import ListTasks from '../components/ListTasks.vue'
import KanbanList from '../components/KanbanList.vue'

const kanbanMode = ref(false)

const handleModeChange = (value: boolean) => {
    kanbanMode.value = value
}
</script>

<template>
  <main class="pt-20 px-4 w-full">
    <Header @mode-changed="handleModeChange" />
    <ListTasks v-if="!kanbanMode" />
    <KanbanList v-else />
  </main>
</template>
