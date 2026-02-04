# Tarefas API

API REST para gerenciamento de tarefas desenvolvida em ASP.NET Core 8.0 com PostgreSQL.

## 📋 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) instalado e rodando
- Um editor de código (Visual Studio, VS Code, Rider, etc.)

## 🗄️ Configuração do Banco de Dados

### 1. Criar o Banco de Dados PostgreSQL

Abra o terminal e conecte-se ao PostgreSQL:

```bash
psql -U postgres
```

Depois, execute o comando para criar o banco de dados:

```sql
CREATE DATABASE tasks;
```

Para sair do psql, digite:
```sql
\q
```

### 2. Configurar a Connection String

A connection string do banco de dados deve ser configurada no arquivo **`appsettings.json`**.

Abra o arquivo `appsettings.json` e configure a connection string no formato:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tasks;Username=seu_usuario;Password=sua_senha;"
  }
}
```

**Exemplo:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tasks;Username=postgres;Password=123;"
  }
}
```

**Parâmetros da connection string:**
- `Host`: Endereço do servidor PostgreSQL (geralmente `localhost`)
- `Port`: Porta do PostgreSQL (padrão: `5432`)
- `Database`: Nome do banco de dados (`tasks`)
- `Username`: Seu usuário do PostgreSQL
- `Password`: Sua senha do PostgreSQL

## Como Executar

### 1. Baixar as Dependências

No diretório raiz do projeto, execute:

```bash
dotnet restore
```

Este comando baixa todas as dependências NuGet necessárias para o projeto.

### 2. Executar as Migrations

Se você fizer alterações nos modelos e precisar criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration
```

Para criar/atualizar as tabelas no banco de dados, execute:

```bash
dotnet ef database update
```

**Nota:** Se você ainda não tem o Entity Framework Tools instalado globalmente, instale com:

```bash
dotnet tool install --global dotnet-ef
```

### 3. Rodar a Aplicação

Para iniciar a aplicação, execute:

```bash
dotnet run
```

Ou, se preferir executar em modo watch (reinicia automaticamente ao detectar mudanças):

```bash
dotnet watch run
```

A aplicação estará disponível em:
- **HTTP:** `http://localhost:5055`
- **HTTPS:** `https://localhost:5055`
- **Swagger UI:** `https://localhost:5055/swagger` (apenas em ambiente de desenvolvimento)

## 📝 Comandos Úteis

### Criar uma Nova Migration

Se você fizer alterações nos modelos e precisar criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration
```

### Remover a Última Migration

Se você criou uma migration por engano:

```bash
dotnet ef migrations remove
```

### Verificar o Status das Migrations

Para ver quais migrations foram aplicadas:

```bash
dotnet ef migrations list
```

## 🗂️ Estrutura do Projeto

```
tarefasApi/
├── Controllers/          # Controllers da API
│   └── TaskController.cs
├── Data/                 # Seeders e dados iniciais
│   └── DataSeeder.cs
├── Migrations/           # Migrations do Entity Framework
├── Models/               # Modelos de dados
│   ├── AppDbContext.cs
│   ├── Tarefa.cs
│   └── TarefaDto.cs
├── Program.cs            # Configuração principal da aplicação
└── appsettings.json      # Configurações e connection string
```

## 🔧 Tecnologias Utilizadas

- **ASP.NET Core 8.0** - Framework web
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL** - Banco de dados
- **Npgsql** - Driver PostgreSQL para .NET
- **Swagger/OpenAPI** - Documentação da API

---------------------------------------------------------------------------

# FrontTasks

Interface web para gerenciamento de tarefas desenvolvida em Vue 3 com TypeScript e Vite.

## Pré-requisitos

- [Node.js](https://nodejs.org/) versão 20.19.0 ou superior (ou 22.12.0+)
- [pnpm](https://pnpm.io/) (gerenciador de pacotes recomendado) ou npm/yarn
- A API backend rodando (veja o README da API para mais detalhes)

##  Configuração da API

A aplicação precisa se conectar à API backend. Por padrão, a URL da API é:

```
http://localhost:5055/api/tasks
```

### Configurar URL da API

Crie um arquivo `.env` na raiz do projeto (opcional, se quiser usar uma URL diferente):

```env
VITE_API_URL=http://localhost:5055/api/tasks
```

**Nota:** Se você não criar o arquivo `.env`, a aplicação usará a URL padrão configurada no `vite.config.ts`.

## Como Executar

### 1. Baixar as Dependências

No diretório raiz do projeto, execute:

```bash
pnpm install
```

Ou, se preferir usar npm:

```bash
npm install
```

Este comando baixa todas as dependências necessárias para o projeto.

### 2. Rodar a Aplicação em Modo de Desenvolvimento

Para iniciar a aplicação em modo de desenvolvimento com hot-reload:

```bash
pnpm dev
```

Ou com npm:

```bash
npm run dev
```

A aplicação estará disponível em:
- **URL:** `http://localhost:5173` (porta padrão do Vite)

O servidor de desenvolvimento reinicia automaticamente quando você faz alterações nos arquivos.

### 3. Build para Produção

Para criar uma build de produção:

```bash
pnpm build
```

Ou com npm:

```bash
npm run build
```


## 🗂️ Estrutura do Projeto

```
frontTasks/
├── public/              # Arquivos estáticos públicos
│   └── favicon.ico
├── src/
│   ├── assets/          # Recursos estáticos (CSS, imagens)
│   │   ├── base.css
│   │   ├── main.css
│   │   └── logo.svg
│   ├── components/      # Componentes Vue reutilizáveis
│   │   ├── FormTask.vue      # Formulário de criação/edição de tarefas
│   │   ├── Header.vue        # Cabeçalho da aplicação
│   │   ├── KanbanList.vue    # Visualização em formato Kanban
│   │   ├── ListTasks.vue     # Visualização em lista
│   │   └── TaskCard.vue       # Card individual de tarefa
│   ├── router/          # Configuração de rotas
│   │   └── index.ts
│   ├── service/         # Serviços de API
│   │   ├── api.ts            # Cliente HTTP base
│   │   └── tasks.ts          # Serviço de tarefas
│   ├── stores/          # Stores do Pinia (gerenciamento de estado)
│   │   └── counter.ts
│   ├── types/           # Definições de tipos TypeScript
│   │   ├── response.ts
│   │   └── task.ts
│   ├── views/           # Páginas/Views da aplicação
│   │   ├── AboutView.vue
│   │   └── HomeView.vue
│   ├── App.vue          # Componente raiz da aplicação
│   └── main.ts          # Ponto de entrada da aplicação
├── index.html           # HTML principal
├── package.json         # Dependências e scripts
├── vite.config.ts       # Configuração do Vite
├── tailwind.config.js   # Configuração do Tailwind CSS
└── tsconfig.json        # Configuração do TypeScript
```
## 🔧 Tecnologias Utilizadas

- **Vue 3** - Framework JavaScript reativo
- **TypeScript** - Tipagem estática para JavaScript
- **Vite** - Build tool e dev server
- **PrimeVue** - Biblioteca de componentes UI
- **Tailwind CSS** - Framework CSS utility-first
- **Vue Router** - Roteamento para aplicações Vue
- **Phosphor Icons** - Biblioteca de ícones

### Estrutura de Componentes

- **Header**: Contém o título da aplicação e o toggle para alternar entre lista e Kanban
- **ListTasks**: Exibe tarefas em formato de lista com filtros
- **KanbanList**: Exibe tarefas em formato Kanban com drag and drop
- **TaskCard**: Componente individual que representa uma tarefa
- **FormTask**: Modal para criar/editar tarefas

### Serviços

- **api.ts**: Cliente HTTP base que faz requisições para a API
- **tasks.ts**: Serviço específico para operações de tarefas (CRUD)

### Tipos

- **task.ts**: Definições de tipos para Task e TaskRequest
- **response.ts**: Definições de tipos para respostas da API

## 🔗 Integração com a API

A aplicação se comunica com a API backend através de requisições HTTP:

- `GET /getall` - Buscar todas as tarefas
- `GET /getbyid/:id` - Buscar tarefa por ID
- `POST /create` - Criar nova tarefa
- `PUT /update` - Atualizar tarefa
- `DELETE /delete` - Deletar tarefa

Certifique-se de que a API backend está rodando antes de iniciar a aplicação frontend.

--Made by ChatGPT
