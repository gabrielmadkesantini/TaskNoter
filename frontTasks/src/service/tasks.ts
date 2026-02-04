import type { Task, Taskrequest } from '@/types/task';
import api from './api';

const getTasks = async (): Promise<Task[]> => {
    try {
        const response = await api.get<Task[]>('/getall');
        return Array.isArray(response.data) ? response.data : [];
    } catch (error) {
        console.error('Erro ao buscar tarefas:', error);
        return [];
    }
};

const getTaskById = async (id: number): Promise<Task> => {
    const response = await api.get<Task>(`/getbyid/${id}`);
    return response.data;
};

const createTask = async (task: Taskrequest): Promise<Taskrequest> => {
    const response = await api.post<Taskrequest>('/create', { body: JSON.stringify(task) });
    return response.data;
};

const updateTask = async (task: Taskrequest): Promise<Task> => {
    const response = await api.put<Task>(`/update`, { body: JSON.stringify(task) });
    return response.data;
};

const deleteTask = async (id: number, task: Task): Promise<void> => {
    await api.del(`/delete`, { body: JSON.stringify(task) });
    // Delete não retorna dados, apenas confirma que foi executado com sucesso
};

export default { getTasks, getTaskById, createTask, updateTask, deleteTask };