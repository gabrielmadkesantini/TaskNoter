using tarefasApi.Models;

namespace tarefasApi.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Verificar se já existem dados
            if (context.Tarefas.Any())
            {
                return; // Já tem dados, não precisa popular
            }

            var tarefas = new List<Tarefa>
            {
                new Tarefa
                {
                    Titulo = "Implementar autenticação",
                    Descricao = "Criar sistema de login e autenticação JWT",
                    Status = "Pendente",
                    Concluida = false,
                    CriadaEm = DateTime.UtcNow.AddDays(-10)
                },
                new Tarefa
                {
                    Titulo = "Configurar CI/CD",
                    Descricao = "Configurar pipeline de deploy automático",
                    Status = "Em Andamento",
                    Concluida = false,
                    CriadaEm = DateTime.UtcNow.AddDays(-8)
                },
                new Tarefa
                {
                    Titulo = "Escrever testes unitários",
                    Descricao = "Criar testes para os controllers e serviços",
                    Status = "Pendente",
                    Concluida = false,
                    CriadaEm = DateTime.UtcNow.AddDays(-6)
                },
                new Tarefa
                {
                    Titulo = "Documentar API",
                    Descricao = "Criar documentação Swagger completa",
                    Status = "Concluído",
                    Concluida = true,
                    CriadaEm = DateTime.UtcNow.AddDays(-15),
                    ConcluidaEm = DateTime.UtcNow.AddDays(-5)
                },
                new Tarefa
                {
                    Titulo = "Otimizar queries",
                    Descricao = "Melhorar performance das consultas ao banco",
                    Status = "Pendente",
                    Concluida = false,
                    CriadaEm = DateTime.UtcNow.AddDays(-4)
                },
                new Tarefa
                {
                    Titulo = "Implementar validações",
                    Descricao = "Adicionar validações de entrada nos endpoints",
                    Status = "Em Andamento",
                    Concluida = false,
                    CriadaEm = DateTime.UtcNow.AddDays(-3)
                },
                new Tarefa
                {
                    Titulo = "Configurar logging",
                    Descricao = "Implementar sistema de logs estruturado",
                    Status = "Pendente",
                    Concluida = false,
                    CriadaEm = DateTime.UtcNow.AddDays(-2)
                },
                new Tarefa
                {
                    Titulo = "Criar dashboard",
                    Descricao = "Desenvolver interface de administração",
                    Status = "Concluído",
                    Concluida = true,
                    CriadaEm = DateTime.UtcNow.AddDays(-12),
                    ConcluidaEm = DateTime.UtcNow.AddDays(-1)
                },
                new Tarefa
                {
                    Titulo = "Revisar código",
                    Descricao = "Fazer code review e refatoração",
                    Status = "Pendente",
                    Concluida = false,
                    CriadaEm = DateTime.UtcNow.AddDays(-1)
                },
                new Tarefa
                {
                    Titulo = "Preparar deploy",
                    Descricao = "Configurar ambiente de produção",
                    Status = "Pendente",
                    Concluida = false,
                    CriadaEm = DateTime.UtcNow
                }
            };

            await context.Tarefas.AddRangeAsync(tarefas);
            await context.SaveChangesAsync();
        }
    }
}
