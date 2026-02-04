<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import TaskCard from './TaskCard.vue'
import type { Task, Taskrequest } from '@/types/task'
import tasksService from '@/service/tasks'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import DatePicker from 'primevue/datepicker'
import FormTask from './FormTask.vue'
import { PhNoteBlank, PhMagnifyingGlass, PhCalendar } from '@phosphor-icons/vue'

const taskStatus = ref<string[]>(['Pendente', 'Em Andamento', 'Concluído'])
const tasks = ref<Task[]>([])
const visible = ref<boolean>(false)
const draggedTask = ref<Task | null>(null)
const draggedOverColumn = ref<string | null>(null)
const searchText = ref<string>('')
const dateRange = ref<Date[] | null>(null)
const dateFilterType = ref<'criacao' | 'fechamento'>('criacao')

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

const getTasksByStatus = (status: string) => {
    return tasks.value.filter(task => {
        // Filtro por status
        if (task.status !== status) {
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
}

const clearFilters = () => {
    searchText.value = ''
    dateRange.value = null
}

const handleDragStart = (event: DragEvent, task: Task) => {
    draggedTask.value = task
    if (event.dataTransfer) {
        event.dataTransfer.effectAllowed = 'move'
        event.dataTransfer.setData('text/plain', String(task.id))
    }
    if (event.target) {
        (event.target as HTMLElement).style.opacity = '0.5'
    }
}

const handleDragEnd = (event: DragEvent) => {
    draggedTask.value = null
    draggedOverColumn.value = null
    if (event.target) {
        (event.target as HTMLElement).style.opacity = '1'
    }
}

const handleDragOver = (event: DragEvent, status: string) => {
    event.preventDefault()
    if (event.dataTransfer) {
        event.dataTransfer.dropEffect = 'move'
    }
    draggedOverColumn.value = status
}

const handleDragLeave = () => {
    draggedOverColumn.value = null
}

const handleDrop = async (event: DragEvent, newStatus: string) => {
    event.preventDefault()
    draggedOverColumn.value = null
    
    if (!draggedTask.value) return
    
    if (draggedTask.value.status === newStatus) {
        draggedTask.value = null
        return
    }
    
    try {
        const updatedTask: Task = {
            id: draggedTask.value.id,
            titulo: draggedTask.value.titulo,
            descricao: draggedTask.value.descricao,
            status: newStatus,
            concluida: draggedTask.value.concluida,
            criadaEm: draggedTask.value.criadaEm
        }
        
        await tasksService.updateTask(updatedTask as Taskrequest)
        
        await loadTasks()
    } catch (error) {
        console.error('Erro ao atualizar status da tarefa:', error)
    } finally {
        draggedTask.value = null
    }
}

onMounted(async () => {
    await loadTasks()
})
</script>

<template>
    <div class="kanban-container">
        <div class="kanban-header">
            <h2 class="kanban-title">Quadro Kanban</h2>
            <Button 
                icon="pi pi-plus" 
                size="small" 
                class="add-task-button" 
                @click="openCreateTaskDialog" 
                style="background-color: #6b7280; color: white; border: none;" 
            />
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
        
        <div class="kanban-board">
            <div 
                v-for="status in taskStatus" 
                :key="status" 
                class="kanban-column"
                :class="{ 'drag-over': draggedOverColumn === status }"
                @dragover="handleDragOver($event, status)"
                @dragleave="handleDragLeave"
                @drop="handleDrop($event, status)"
            >
                <div class="column-header">
                    <h3 class="column-title">{{ status }}</h3>
                    <span class="column-count">{{ getTasksByStatus(status).length }}</span>
                </div>
                
                <div class="column-content">
                    <TransitionGroup name="kanban-list" tag="div">
                        <div
                            v-for="task in getTasksByStatus(status)" 
                            :key="task.id as number"
                            class="kanban-card-wrapper"
                            draggable="true"
                            @dragstart="handleDragStart($event, task)"
                            @dragend="handleDragEnd($event)"
                        >
                            <TaskCard 
                                :task="task"
                                class="kanban-task-card"
                                @task-updated="loadTasks"
                            />
                        </div>
                    </TransitionGroup>
                    
                    <div v-if="getTasksByStatus(status).length === 0" class="empty-column">
                        <PhNoteBlank :size="32" weight="thin" class="empty-icon" />
                        <p class="empty-text">Nenhuma tarefa</p>
                    </div>
                </div>
            </div>
        </div>

        <FormTask v-model:visible="visible" @task-saved="loadTasks" />
    </div>
</template>

<style scoped>
.kanban-container {
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

.kanban-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 100%;
    padding-bottom: 1rem;
    border-bottom: 2px solid rgba(226, 232, 240, 0.8);
}

.kanban-title {
    font-size: 1.5rem;
    font-weight: 700;
    color: #1e293b;
    margin: 0;
}

.kanban-board {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1.5rem;
    width: 100%;
    min-height: 400px;
}

.kanban-column {
    display: flex;
    flex-direction: column;
    background: white;
    border-radius: 0.75rem;
    padding: 1rem;
    box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1);
    min-height: 400px;
    max-height: calc(100vh - 250px);
    overflow: hidden;
}

.column-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding-bottom: 1rem;
    margin-bottom: 1rem;
    border-bottom: 2px solid #e2e8f0;
}

.column-title {
    font-size: 1rem;
    font-weight: 600;
    color: #475569;
    margin: 0;
}

.column-count {
    display: flex;
    align-items: center;
    justify-content: center;
    min-width: 24px;
    height: 24px;
    padding: 0 8px;
    background-color: #e2e8f0;
    border-radius: 12px;
    font-size: 0.75rem;
    font-weight: 600;
    color: #64748b;
}

.column-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    overflow-y: auto;
    flex: 1;
    padding-right: 0.25rem;
}

.column-content::-webkit-scrollbar {
    width: 6px;
}

.column-content::-webkit-scrollbar-track {
    background: #f1f5f9;
    border-radius: 3px;
}

.column-content::-webkit-scrollbar-thumb {
    background: #cbd5e1;
    border-radius: 3px;
}

.column-content::-webkit-scrollbar-thumb:hover {
    background: #94a3b8;
}

.kanban-card-wrapper {
    cursor: grab;
    margin-bottom: 1.25rem !important;
    transition: transform 0.2s ease, opacity 0.2s ease;
}

.kanban-card-wrapper:last-child {
    margin-bottom: 0 !important;
}

.kanban-card-wrapper:active {
    cursor: grabbing;
}

.kanban-card-wrapper:hover {
    transform: translateY(-2px);
}

.kanban-task-card {
    margin-bottom: 0 !important;
}

.kanban-task-card .task-card {
    margin-bottom: 0 !important;
}

.kanban-column.drag-over {
    background-color: #f0f9ff;
    border: 2px dashed #3b82f6;
}

.kanban-column.drag-over .column-content {
    background-color: transparent;
}

.empty-column {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: 2rem 1rem;
    text-align: center;
    color: #94a3b8;
}

.empty-icon {
    opacity: 0.4;
    margin-bottom: 0.5rem;
}

.empty-text {
    font-size: 0.875rem;
    color: #94a3b8;
    margin: 0;
}

/* Transitions */
.kanban-list-enter-active {
    transition: all 0.3s ease-out;
}

.kanban-list-leave-active {
    transition: all 0.2s ease-in;
}

.kanban-list-enter-from {
    opacity: 0;
    transform: translateY(10px) scale(0.95);
}

.kanban-list-leave-to {
    opacity: 0;
    transform: translateY(-10px) scale(0.95);
}

.kanban-list-move {
    transition: transform 0.3s ease;
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

/* Responsive */
@media (max-width: 1024px) {
    .kanban-board {
        grid-template-columns: 1fr;
    }
}
</style>
