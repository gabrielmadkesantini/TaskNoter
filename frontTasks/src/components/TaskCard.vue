<script setup lang="ts">
import { Button, Dialog } from 'primevue'
import { PhListChecks, PhCalendar, PhDotsThree, PhPencilSimple, PhTrash, PhWarning } from '@phosphor-icons/vue'
import { computed, ref } from 'vue'
import FormTask from './FormTask.vue'
import type { Task } from '@/types/task'
import tasksService from '@/service/tasks'

interface Props {
  task: Task
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'task-updated'): void
}>()

const formattedDate = computed(() => {
  if (!props.task.criadaEm) return 'Não definida'
  try {
    const date = new Date(props.task.criadaEm)
    if (isNaN(date.getTime())) return 'Data inválida'
    return date.toLocaleDateString('pt-BR', { 
      day: '2-digit', 
      month: 'short', 
      year: 'numeric' 
    })
  } catch (error) {
    return 'Data inválida'
  }
})

const formattedConcluidaDate = computed(() => {
  if (!props.task.concluidaEm) return null
  try {
    const date = new Date(props.task.concluidaEm)
    if (isNaN(date.getTime())) return null
    return date.toLocaleDateString('pt-BR', { 
      day: '2-digit', 
      month: 'short', 
      year: 'numeric' 
    })
  } catch (error) {
    return null
  }
})

const visible = ref(false)
const isDeleting = ref(false)
const showDeleteConfirm = ref(false)
const showErrorDialog = ref(false)
const errorMessage = ref('')

const openFormTask = () => {
    console.log('openFormTask')
    visible.value = true
}

const openDeleteConfirm = () => {
    showDeleteConfirm.value = true
}

const confirmDelete = async () => {
    if (!props.task.id) {
        console.error('Tarefa sem ID')
        return
    }

    showDeleteConfirm.value = false
    isDeleting.value = true
    
    try {
        await tasksService.deleteTask(props.task.id, props.task)
        emit('task-updated')
    } catch (error) {
        console.error('Erro ao deletar tarefa:', error)
        errorMessage.value = 'Erro ao deletar a tarefa. Tente novamente.'
        showErrorDialog.value = true
    } finally {
        isDeleting.value = false
    }
}

const cancelDelete = () => {
    showDeleteConfirm.value = false
}

</script>

<template>
  <div :class="{ 'task-concluida': task.concluida }" class="task-card flex mb- justify-start flex-col w-full gap-3 rounded-lg p-4 cursor-pointer bg-white border border-gray-200 shadow-sm hover:shadow-md transition-all duration-200">
    <div class="flex items-start justify-center ">
      <div class="flex items-center gap-2 flex-1">
        
        <p class="text-md font-semibold text-gray-800 task-title">{{ task.titulo }}</p>
      </div>
      <Button type="button" @click="openFormTask" aria-haspopup="true" aria-controls="overlay_menu" class="p-2" text rounded>
        <PhPencilSimple :size="22" weight="light" color="#2563eb" />
      </Button>
      <Button 
        type="button" 
        @click="openDeleteConfirm" 
        aria-haspopup="true" 
        aria-controls="overlay_menu" 
        class="p-2" 
        text 
        rounded
        :disabled="isDeleting"
      >
        <PhTrash :size="22" weight="light" color="#ef4444" />
      </Button>
    </div>
    <FormTask 
      :visible="visible" 
      :task="task"
      @update:visible="visible = $event"
      @task-saved="emit('task-updated')"
    />

    <!-- Modal de Confirmação de Exclusão -->
    <Dialog 
      v-model:visible="showDeleteConfirm" 
      modal 
      header="Confirmar Exclusão"
      :style="{ width: '90vw', maxWidth: '500px' }"
      :draggable="false"
      class="delete-confirm-dialog"
    >
      <div class="delete-confirm-content">
        <div class="delete-confirm-icon">
          <PhWarning :size="48" weight="fill" color="#ef4444" />
        </div>
        <p class="delete-confirm-message">
          Tem certeza que deseja excluir a tarefa <strong>"{{ task.titulo }}"</strong>?
        </p>
        <p class="delete-confirm-warning">
          Esta ação não pode ser desfeita.
        </p>
      </div>
      <template #footer>
        <div class="delete-confirm-actions">
          <Button 
            label="Cancelar" 
            icon="pi pi-times" 
            @click="cancelDelete" 
            :disabled="isDeleting"
            text 
            severity="secondary" 
            size="small"
          />
          <Button 
            label="Excluir" 
            icon="pi pi-trash" 
            @click="confirmDelete" 
            :loading="isDeleting"
            :disabled="isDeleting"
            severity="danger" 
            size="small"
          />
        </div>
      </template>
    </Dialog>

    <!-- Modal de Erro -->
    <Dialog 
      v-model:visible="showErrorDialog" 
      modal 
      header="Erro"
      :style="{ width: '90vw', maxWidth: '500px' }"
      :draggable="false"
      class="error-dialog"
    >
      <div class="error-dialog-content">
        <div class="error-icon">
          <i class="pi pi-exclamation-circle" style="font-size: 2rem; color: #ef4444;"></i>
        </div>
        <p class="error-message">{{ errorMessage }}</p>
      </div>
      <template #footer>
        <Button 
          label="Fechar" 
          icon="pi pi-check" 
          @click="showErrorDialog = false" 
          severity="secondary" 
          size="small"
        />
      </template>
    </Dialog>
    
    <p class="text-sm text-gray-600 leading-relaxed task-description">{{ task.descricao || 'Sem descrição' }}</p>
    
    <div class="flex items-center justify-between pt-2 border-t border-gray-100">
      <div class="flex items-center gap-3 flex-wrap text-xs text-gray-500 task-date">
        <div class="flex items-center gap-1.5">
          <PhCalendar :size="14" weight="thin" />
          <span>Criado em {{ formattedDate }}</span>
        </div>
        <div v-if="task.concluida" class="flex items-center gap-1.5 task-concluida-date">
          <PhCalendar :size="14" weight="thin" />
          <span>Concluída em {{ formattedConcluidaDate || 'Data não disponível' }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.task-card {
  transition: opacity 0.3s ease, filter 0.3s ease;
}

.task-concluida {
  opacity: 1;
  filter: grayscale(0.5);
  background-color: #f3f4f6 !important;
}

.task-concluida .task-title {
  text-decoration: line-through;
  color: #9ca3af !important;
}

.task-concluida .task-description {
  color: #9ca3af !important;
}

.task-concluida .task-date {
  color: #9ca3af !important;
}

.task-concluida-date {
  color: #f87171 !important;
}

.task-concluida:hover {
  opacity: 1;
  transform: none;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
}

/* Estilos do Modal de Confirmação */
.delete-confirm-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  padding: 1rem 0;
  text-align: center;
}

.delete-confirm-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background-color: #fef2f2;
  margin-bottom: 0.5rem;
}

.delete-confirm-message {
  font-size: 1rem;
  color: #374151;
  margin: 0;
  line-height: 1.5;
}

.delete-confirm-message strong {
  color: #1f2937;
  font-weight: 600;
}

.delete-confirm-warning {
  font-size: 0.875rem;
  color: #6b7280;
  margin: 0;
}

.delete-confirm-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  width: 100%;
}

/* Estilos do Modal de Erro */
.error-dialog-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  padding: 1rem 0;
  text-align: center;
}

.error-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 0.5rem;
}

.error-message {
  font-size: 1rem;
  color: #374151;
  margin: 0;
  line-height: 1.5;
}
</style>