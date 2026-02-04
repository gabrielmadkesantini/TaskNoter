import type { Response } from '../types/response';

const URL = import.meta.env.VITE_API_URL;

const get = async <T>(path: string, options?: RequestInit): Promise<Response<T>> => {
    const url = `${URL}${path}`;
    const response = await fetch(url, {
        ...options,
        method: 'GET',
    });
    
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    const data = await response.json();
    
    return {
        data,
        status: response.status,
        statusText: response.statusText,
        headers: response.headers,
        request: {} as XMLHttpRequest
    } as Response<T>;
};

const post = async <T>(path: string, options: RequestInit): Promise<Response<T>> => {
    const url = `${URL}${path}`;
    const response = await fetch(url, {
        ...options,
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            ...options?.headers,
        },
    });
    
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    const data = await response.json();
    
    return {
        data,
        status: response.status,
        statusText: response.statusText,
        headers: response.headers,
        request: {} as XMLHttpRequest
    } as Response<T>;
};


const del = async <T>(path: string, options: RequestInit): Promise<Response<T>> => {
    const url = `${URL}${path}`;
    const response = await fetch(url, {
        ...options,
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            ...options?.headers,
        },
    });
    
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    // Verifica se há conteúdo na resposta antes de tentar fazer parse
    const contentType = response.headers.get('content-type');
    const text = await response.text();
    
    let data: T;
    if (text && contentType && contentType.includes('application/json')) {
        try {
            data = JSON.parse(text) as T;
        } catch (error) {
            data = null as T;
        }
    } else {
        data = null as T;
    }
    
    return {
        data,
        status: response.status,
        statusText: response.statusText,
        headers: response.headers,
        request: {} as XMLHttpRequest
    } as Response<T>;
};

const put = async <T>(path: string, options: RequestInit): Promise<Response<T>> => {
    const url = `${URL}${path}`;
    const response = await fetch(url, {
        ...options,
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            ...options?.headers,
        },
    });

    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    const data = await response.json();

    return {
        data,
        status: response.status,
        statusText: response.statusText,
        headers: response.headers,
        request: {} as XMLHttpRequest
    } as Response<T>;
};

const api = {
    get,
    post,
    del,
    put,
};

export default api;
