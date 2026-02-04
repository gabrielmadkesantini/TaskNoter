<script setup lang="ts">
import { Dialog, Button, InputText, FloatLabel, DatePicker, RadioButton, Textarea } from 'primevue'
import { ref, reactive, watch, computed } from 'vue';
import type { Task, Taskrequest } from '@/types/task';
import tasksService from '@/service/tasks';

interface Props {
  visible: boolean
  task?: Task | null
}

const props = withDefaults(defineProps<Props>(), {
  visible: false,
  task: null
})

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'task-saved'): void
}>()

const isLoading = ref(false)
const errorMessage = ref<string | null>(null)
const successMessage = ref<string | null>(null)

const statusOptions = ['Pendente', 'Em Andamento', 'Concluído']

const taskForm = reactive<{
  titulo: string
  descricao: string | null
  status: string | null
  date: string
  concluida: boolean
}>({
  titulo: '',
  descricao: '',
  status: 'Pendente',
  date: '',
  concluida: false
})

// Computed para converter string para Date (para o DatePicker)
const dateValue = computed({
  get: () => {
    if (!taskForm.date || taskForm.date === '') return null
    try {
      return new Date(taskForm.date)
    } catch {
      return null
    }
  },
  set: (value: Date | null) => {
    if (value) {
      taskForm.date = value.toISOString()
    } else {
      taskForm.date = ''
    }
  }
})

const isEditMode = computed(() => props.task?.id !== null && props.task?.id !== undefined)

const resetForm = () => {
  taskForm.titulo = ''
  taskForm.descricao = ''
  taskForm.status = 'Pendente'
  taskForm.date = ''
  errorMessage.value = null
  successMessage.value = null
}

const loadTaskData = async () => {
  if (!props.task || props.task.id === null || props.task.id === undefined) {
    resetForm()
    return
  }

  try {
    const task = await tasksService.getTaskById(props.task.id)
    if (task) {
      taskForm.titulo = task.titulo || ''
      taskForm.descricao = task.descricao || ''
      taskForm.status = task.status || 'Pendente'
      taskForm.date = task.criadaEm || ''
    }
  } catch (error) {
    console.error('Erro ao carregar tarefa:', error)
    resetForm()
  }
}



watch(() => props.visible, async (newValue) => {
  if (newValue) {
    if (isEditMode.value && props.task?.id) {
      await loadTaskData()
    } else {
      resetForm()
    }
  } else {
    resetForm()
  }
})

watch(() => props.task, async () => {
  if (props.visible) {
    if (isEditMode.value && props.task?.id) {
      await loadTaskData()
    } else {
      resetForm()
    }
  }
}, { deep: true })

const closeFormTask = () => {
  emit('update:visible', false)
  resetForm()
}

const validateForm = (): boolean => {
  if (!taskForm.titulo || taskForm.titulo.trim() === '') {  
    errorMessage.value = 'O título é obrigatório'
    return false
  }
  if (taskForm.titulo.length < 3) {
    errorMessage.value = 'O título deve ter pelo menos 3 caracteres'
    return false
  }
  return true
}

const saveTask = async () => {
  if (!validateForm()) {
    return
  }

  isLoading.value = true
  errorMessage.value = null
  successMessage.value = null

  try {
    // A data já está como string ISO no taskForm.date
    const dateToSend: string | null = taskForm.date && taskForm.date !== '' ? taskForm.date : null
    
    const taskData: Task = {
      id: props.task?.id || null,
      titulo: taskForm.titulo.trim(),
      descricao: taskForm.descricao?.trim() || null,
      status: taskForm.status || null,
      concluida: taskForm.concluida || false,
    }

    const taskRequest: Taskrequest = {
      titulo: taskForm.titulo.trim(),
      descricao: taskForm.descricao?.trim() || null,
      status: taskForm.status || null,
      concluida: taskForm.concluida || false,
    }

    if (isEditMode.value && props.task?.id) {
      await tasksService.updateTask(taskData)
      successMessage.value = 'Tarefa atualizada com sucesso!'
    } else {
      await tasksService.createTask(taskRequest)
      successMessage.value = 'Tarefa criada com sucesso!'
    }

    // Aguardar um pouco para mostrar a mensagem de sucesso
    setTimeout(() => {
      emit('task-saved')
      closeFormTask()
    }, 1000)
  } catch (error) {
    console.error('Erro ao salvar tarefa:', error)
    errorMessage.value = isEditMode.value 
      ? 'Erro ao atualizar a tarefa. Tente novamente.' 
      : 'Erro ao criar a tarefa. Tente novamente.'
  } finally {
    isLoading.value = false
  }
}

</script>

<template>
  <Dialog 
    v-model:visible="props.visible" 
    @update:visible="emit('update:visible', $event)" 
    modal 
    :header="isEditMode ? 'Editar Task' : 'Criar Task'"
    :style="{ width: '90vw', maxWidth: '600px' }"
    :draggable="false"
    class="task-form-dialog"
  >
    <div class="form-container">
      <p class="form-subtitle">
        {{ isEditMode ? 'Edite os dados da tarefa' : 'Preencha os dados para criar uma nova tarefa' }}
      </p>

      <!-- Mensagens de feedback -->
      <div 
        v-if="errorMessage" 
        class="message message-error"
      >
        <i class="pi pi-exclamation-circle"></i>
        <span>{{ errorMessage }}</span>
      </div>
      
      <div 
        v-if="successMessage" 
        class="message message-success"
      >
        <i class="pi pi-check-circle"></i>
        <span>{{ successMessage }}</span>
      </div>

      <div class="form-fields">
        <!-- Título -->
        <div>
          <FloatLabel variant="on">
            <InputText 
              id="title" 
              autocomplete="off"                    
              size="small" 
              name="title" 
              v-model="taskForm.titulo"
              :disabled="isLoading"
              class="w-full"
            />
            <label for="title">Título *</label>
          </FloatLabel>
        </div>

        <!-- Descrição -->
        <div >
          <FloatLabel variant="on">
            <Textarea
              id="description" 
              autocomplete="off" 
              size="small" 
              name="description" 
              v-model="taskForm.descricao"
              :disabled="isLoading"
              class="w-full"
            />
            <label for="description">Descrição</label>
          </FloatLabel>
        </div>

        <!-- Data -->
        <div class="field-wrapper">
          <FloatLabel variant="on">
            <DatePicker 
              id="date" 
              size="small" 
              showIcon 
              showTime 
              iconDisplay="input" 
              hourFormat="12" 
              name="date" 
              v-model="dateValue"
              :disabled="isLoading"
              class="w-full"
              dateFormat="dd/mm/yy" />
            <label for="date">Data de Conclusão</label>
          </FloatLabel>
        </div>

        <!-- Status -->
        <div class="field-wrapper">
          <label for="status" class="status-label">Status *</label>
          <div class="status-options">
            <div 
              v-for="statusItem in statusOptions" 
              :key="statusItem" 
              class="status-option"
            >
              <RadioButton 
                :id="`status-${statusItem}`" 
                name="status" 
                v-model="taskForm.status"
                color="primary" 
                :value="statusItem"
                :disabled="isLoading"
              />
              <label 
                :for="`status-${statusItem}`" 
                class="status-option-label"
              >
                {{ statusItem }}
              </label>
            </div>
          </div>
        </div>
      </div>

      <!-- Botões de ação -->
      <div class="form-actions">
        <Button 
          :label="isEditMode ? 'Salvar' : 'Criar'" 
          icon="pi pi-check" 
          @click="saveTask" 
          :loading="isLoading"
          :disabled="isLoading"
          class="save-button"
          severity="primary" 
          size="small"
        />
        <Button 
          type="button" 
          label="Cancelar" 
          icon="pi pi-times" 
          @click="closeFormTask" 
          :disabled="isLoading"
          text 
          severity="secondary" 
          size="small"
        />
      </div>
    </div>
  </Dialog>
</template>

<style scoped>
.form-container {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  padding: 0.5rem 0;
}

.form-subtitle {
  color: #64748b;
  font-size: 0.875rem;
  margin: 0;
  margin-bottom: 0.5rem;
}

.form-fields {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.p-textarea {
  color: black !important;
  border-radius: 0.375rem !important;
  min-width: 400px !important;
  border: 1px solid rgb(190, 190, 190) !important;
  min-height: 100px !important;
}

.p-textarea:focus-visible {
  color: black !important;
  border-radius: 0.375rem !important;
}

.status-label {
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
  margin-bottom: 0.5rem;
  display: block;
}

.status-options {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
  align-items: center;
}

.status-option {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  transition: all 0.2s ease;
  cursor: pointer;
}


.status-option-label {
  font-size: 0.875rem;
  color: #475569;
  cursor: pointer;
  margin: 0;
  user-select: none;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding-top: 0.5rem;
  border-top: 1px solid #e2e8f0;
}

.save-button {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: none;
  color: white;
  transition: all 0.2s ease;
}

.save-button:hover:not(:disabled) {
  background: linear-gradient(135deg, #5568d3 0%, #6a3f8f 100%);
  transform: translateY(-1px);
  box-shadow: 0 4px 6px -1px rgba(102, 126, 234, 0.3);
}

.save-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

:deep(.p-dialog-header) {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 6px 6px 0 0;
}

:deep(.p-dialog-header-icon) {
  color: white;
}

:deep(.p-dialog-title) {
  color: white;
  font-weight: 600;
}

:deep(.p-float-label label) {
  color: #64748b;
}



:deep(.p-radiobutton-box.p-highlight) {
  background-color: #2563eb;
  border-color: #2563eb;
}

:deep(.p-radiobutton-box.p-highlight .p-radiobutton-icon) {
  background-color: #2563eb;
}

:deep(.p-radiobutton-box:not(.p-disabled):hover) {
  border-color: #2563eb;
}

:deep(.p-datepicker) {
  width: 100%;
}

:deep(.p-datepicker-input) {
  width: 100%;
}

.message {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  margin-bottom: 1rem;
}

.message-error {
  background-color: #fee2e2;
  color: #991b1b;
  border: 1px solid #fecaca;
}

.message-success {
  background-color: #d1fae5;
  color: #065f46;
  border: 1px solid #a7f3d0;
}

.message i {
  font-size: 1rem;
}
</style>
