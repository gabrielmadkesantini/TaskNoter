<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import TaskCard from './TaskCard.vue'
import type { Task } from '@/types/task'
import tasksService from '@/service/tasks'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import DatePicker from 'primevue/datepicker'
import FormTask from './FormTask.vue'
import { PhList, PhNoteBlank, PhMagnifyingGlass, PhCalendar } from '@phosphor-icons/vue'

const taskStatus = ref<string[]>(['Pendente', 'Em Andamento', 'Concluído'])
const activeStatuses = ref<Set<string>>(new Set(['Pendente', 'Em Andamento', 'Concluído']))
const tasks = ref<Task[]>([])
const visible = ref<boolean>(false)
const searchText = ref<string>('')
const dateRange = ref<Date[] | null>(null)
const dateFilterType = ref<'criacao' | 'fechamento'>('criacao')

const changeTaskStatus = (status: string) => {
    if (activeStatuses.value.has(status)) {
        activeStatuses.value.delete(status)
    } else {
        activeStatuses.value.add(status)
    }
}

const isStatusActive = (status: string) => {
    return activeStatuses.value.has(status)
}

const filteredTasks = computed(() => {
    return tasks.value.filter(task => {
        // Filtro por status
        if (!task.status || !activeStatuses.value.has(task.status)) {
            return false
        }

        // Filtro por texto (título ou descrição)
        if (searchText.value.trim()) {
            const searchLower = searchText.value.toLowerCase().trim()
            const titleMatch = task.titulo?.toLowerCase().includes(searchLower) || false
            const descMatch = task.descricao?.toLowerCase().includes(searchLower) || false
            if (!titleMatch && !descMatch) {
                return false
            }
        }

        // Filtro por data
        if (dateRange.value && dateRange.value.length === 2) {
            const [startDate, endDate] = dateRange.value
            if (startDate && endDate) {
                const start = new Date(startDate)
                start.setHours(0, 0, 0, 0)
                const end = new Date(endDate)
                end.setHours(23, 59, 59, 999)

                let taskDate: Date | null = null

                if (dateFilterType.value === 'criacao' && task.criadaEm) {
                    taskDate = new Date(task.criadaEm)
                } else if (dateFilterType.value === 'fechamento' && task.concluidaEm) {
                    taskDate = new Date(task.concluidaEm)
                }

                if (taskDate) {
                    taskDate.setHours(0, 0, 0, 0)
                    if (taskDate < start || taskDate > end) {
                        return false
                    }
                } else {
                    // Se não tem a data solicitada, não mostra a tarefa
                    return false
                }
            }
        }

        return true
    })
})

const clearFilters = () => {
    searchText.value = ''
    dateRange.value = null
}

const openCreateTaskDialog = () => {
    visible.value = true
}

const loadTasks = async () => {
    try {
        const result = await tasksService.getTasks()
        tasks.value = result
    } catch (error) {
        console.error('Erro ao carregar tarefas:', error)
        tasks.value = []
    }
}

onMounted(async () => {
    await loadTasks()
})

</script>

<template>
    <div class="tasks-container">
        <div class="tasks-header">
        <div class="status-filters">
             <div v-for="status in taskStatus"  @click="changeTaskStatus(status)"
             :key="status" 
             :class="{ 'active': isStatusActive(status) }" 
             class="status-filter">
                <p class="status-text">{{ status }}</p>
             </div>
        </div>
        <Button icon="pi pi-plus" size="small" class="add-task-button" @click="openCreateTaskDialog" style="background-color: #6b7280; color: white; border: none;" />
        </div>
        
        <div class="filters-section">
            <div class="filter-group">
                <label class="filter-label">
                    <PhMagnifyingGlass :size="16" weight="regular" />
                    Buscar por título ou descrição
                </label>
                <InputText 
                    v-model="searchText" 
                    placeholder="Digite para buscar..."
                    class="search-input"
                    size="small"
                />
            </div>
            
            <div class="filter-group">
                <label class="filter-label">
                    <PhCalendar :size="16" weight="regular" />
                    Filtrar por data
                </label>
                <div class="date-filter-controls">
                    <select v-model="dateFilterType" class="date-type-select">
                        <option value="criacao">Data de Criação</option>
                        <option value="fechamento">Data de Conclusão</option>
                    </select>
                    <DatePicker 
                        v-model="dateRange" 
                        selectionMode="range" 
                        :manualInput="false"
                        showIcon
                        iconDisplay="input"
                        dateFormat="dd/mm/yy"
                        placeholder="Selecione o intervalo"
                        class="date-range-picker"
                        size="small"
                    />
                </div>
            </div>
            
            <Button 
                v-if="searchText || dateRange"
                label="Limpar Filtros" 
                icon="pi pi-times" 
                @click="clearFilters"
                size="small"
                text
                severity="secondary"
                class="clear-filters-button"
            />
        </div>
        
        <div class="tasks-list">
            <TransitionGroup name="task-list" tag="div">
                <TaskCard 
                    v-for="(task, index) in filteredTasks" 
                    :key="task.id as number" 
                    :task="task"
                    :style="{ '--index': index }"
                    class="task-item"
                    @task-updated="loadTasks"
                />
            </TransitionGroup>
            
            <div v-if="!filteredTasks || filteredTasks.length === 0" class="empty-state">
                <PhNoteBlank :size="64" weight="thin" class="empty-icon" />
                <p class="empty-message">Nenhuma tarefa encontrada</p>
                <p class="empty-submessage">Clique no botão + para criar sua primeira tarefa</p>
            </div>
        </div>

        <FormTask v-model:visible="visible" @task-saved="loadTasks" />
    </div>
</template>

<style scoped>
.tasks-container {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  padding: 1.5rem;
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  border-radius: 1rem;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  height: auto;
  animation: fadeInUp 0.6s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.tasks-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  width: 100%;
  padding-bottom: 1rem;
  border-bottom: 2px solid rgba(226, 232, 240, 0.8);
  animation: fadeInDown 0.5s ease-out;
}

@keyframes fadeInDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.status-filters {
  display: flex;
  gap: 0.75rem;
  align-items: center;
  justify-content: flex-start;
  flex-wrap: wrap;
}

.status-filter {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 9999px;
  cursor: pointer;
  background-color: #ffffff;
  border: 1.5px solid #e2e8f0;
  transition: all 0.2s ease-in-out;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  animation: slideInLeft 0.5s ease-out backwards;
}

.status-filter:nth-child(1) {
  animation-delay: 0.1s;
}

.status-filter:nth-child(2) {
  animation-delay: 0.2s;
}

.status-filter:nth-child(3) {
  animation-delay: 0.3s;
}

@keyframes slideInLeft {
  from {
    opacity: 0;
    transform: translateX(-20px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.status-filter:hover {
  background-color: #f8fafc;
  border-color: #cbd5e1;
  transform: translateY(-1px);
  box-shadow: 0 2px 4px 0 rgba(0, 0, 0, 0.1);
}

.status-filter.active {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-color: #667eea;
  color: white;
  box-shadow: 0 4px 6px -1px rgba(102, 126, 234, 0.3);
}

.status-filter.active .status-text {
  color: white;
  font-weight: 600;
}

.status-text {
  font-size: 0.75rem;
  color: #475569;
  font-weight: 500;
  white-space: nowrap;
  margin: 0;
}

.tasks-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  width: 100%;
  min-height: 200px;
}

.tasks-list > div {
  display: flex;
  flex-direction: column;
}

.task-item .task-card {
  margin-bottom: 0 !important;
}

.task-item {
  animation: slideInRight 0.5s ease-out backwards;
  margin-bottom: 1.25rem !important;
}

.task-item:last-child {
  margin-bottom: 0 !important;
}

.task-item:nth-child(1) {
  animation-delay: 0.1s;
}

.task-item:nth-child(2) {
  animation-delay: 0.2s;
}

.task-item:nth-child(3) {
  animation-delay: 0.3s;
}

.task-item:nth-child(4) {
  animation-delay: 0.4s;
}

.task-item:nth-child(5) {
  animation-delay: 0.5s;
}

.task-item:nth-child(n+6) {
  animation-delay: 0.6s;
}

@keyframes slideInRight {
  from {
    opacity: 0;
    transform: translateX(20px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

/* Transitions para adicionar/remover tasks */
.task-list-enter-active {
  transition: all 0.4s ease-out;
}

.task-list-leave-active {
  transition: all 0.3s ease-in;
}

.task-list-enter-from {
  opacity: 0;
  transform: translateX(30px) scale(0.95);
}

.task-list-leave-to {
  opacity: 0;
  transform: translateX(-30px) scale(0.95);
}

.task-list-move {
  transition: transform 0.4s ease;
}

/* Empty state */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
  animation: fadeIn 0.6s ease-out;
}

.empty-icon {
  color: #94a3b8;
  margin-bottom: 1rem;
  opacity: 0.6;
  animation: float 3s ease-in-out infinite;
}

@keyframes float {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-10px);
  }
}

.empty-message {
  font-size: 1.25rem;
  font-weight: 600;
  color: #475569;
  margin-bottom: 0.5rem;
}

.empty-submessage {
  font-size: 0.875rem;
  color: #64748b;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.add-task-button:hover {
  background-color: #4b5563 !important;
  transform: scale(1.05);
  transition: all 0.2s ease-in-out;
}

/* Filtros */
.filters-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 1rem;
  background: white;
  border-radius: 0.75rem;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1);
  margin-bottom: 1rem;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.filter-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #475569;
}

.search-input {
  width: 100%;
}

.date-filter-controls {
  display: flex;
  gap: 0.75rem;
  align-items: center;
  flex-wrap: wrap;
}

.date-type-select {
  padding: 0.5rem;
  border: 1px solid #e2e8f0;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  color: #475569;
  background: white;
  cursor: pointer;
  transition: border-color 0.2s;
}

.date-type-select:hover {
  border-color: #cbd5e1;
}

.date-type-select:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.date-range-picker {
  flex: 1;
  min-width: 250px;
}

.clear-filters-button {
  align-self: flex-start;
}
</style>