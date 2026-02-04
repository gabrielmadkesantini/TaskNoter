type Taskrequest = {
    
    titulo: string
    descricao: string | null
    status: string | null
    concluida: boolean
    criadaEm?: string | null
    concluidaEm?: string | null
}

type Task = Taskrequest & {
    id: number | null
}

export type { Task, Taskrequest }