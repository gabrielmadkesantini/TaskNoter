using Microsoft.AspNetCore.Mvc;
using tarefasApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace tarefasApi.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        private static DateTime ToUtc(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;
            
            if (dateTime.Kind == DateTimeKind.Local)
                return dateTime.ToUniversalTime();
            
            // Unspecified - assumir que é UTC (comum em JSON ISO strings)
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
        [HttpGet("getall")]
        public async Task<ActionResult<IEnumerable<Tarefa>>> GetTasks()
        {
            var tasks = await _context.Tarefas.ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult<Tarefa>> GetTaskById(int id)
        {
            var task = await _context.Tarefas.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPost("create")]
        public async Task<ActionResult<Tarefa>> CreateTask()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                
                Console.WriteLine("Body recebido (CREATE): " + body);
                
                if (string.IsNullOrWhiteSpace(body))
                {
                    return BadRequest("Body is null or empty");
                }
                
                // Fazer o parse do JSON para o DTO
                var dto = JsonSerializer.Deserialize<TarefaDto>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (dto == null)
                {
                    return BadRequest("Failed to deserialize task");
                }
                
                if (dto.Id != 0)
                {
                    return BadRequest("Id must be 0 for creating a new task. Use PUT /api/tasks/update to update an existing task.");
                }
                
                Console.WriteLine($"DTO parsed (CREATE) - Descricao: {dto.Descricao}, Titulo: {dto.Titulo}");
                
                // Converter data para UTC (o postgres exige UTC)
                DateTime criadaEm = dto.CriadaEm != null 
                    ? ToUtc(dto.CriadaEm)
                    : DateTime.UtcNow;
                
                var task = new Tarefa
                {
                    Id = 0, // Será gerado automaticamente pelo banco
                    Descricao = dto.Descricao ?? dto.Titulo ?? string.Empty,
                    Titulo = dto.Titulo ?? string.Empty,
                    Status = dto.Status ?? "Pendente",
                    Concluida = false,
                    CriadaEm = criadaEm
                };
                
                if (string.IsNullOrWhiteSpace(task.Descricao))
                {
                    return BadRequest("Descricao or Titulo is required");
                }
                
                _context.Tarefas.Add(task);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"Task created with Id: {task.Id}");
                return Ok(task);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Parse Error: {ex.Message}");
                return BadRequest($"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult<Tarefa>> UpdateTask()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                
                Console.WriteLine("Body recebido (UPDATE): " + body);
                
                if (string.IsNullOrWhiteSpace(body))
                {
                    return BadRequest("Body is null or empty");
                }
                
                // Fazer o parse do JSON para o DTO
                var dto = JsonSerializer.Deserialize<TarefaDto>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (dto == null)
                {
                    return BadRequest("Failed to deserialize task");
                }
                
                if (dto.Id == 0)
                {
                    return BadRequest("Id is required for updating a task. Use POST /api/tasks/create to create a new task.");
                }
                
                Console.WriteLine($"DTO parsed (UPDATE) - Id: {dto.Id}, Descricao: {dto.Descricao}, Titulo: {dto.Titulo}");
                
                // Buscar tarefa existente
                var existingTask = await _context.Tarefas.FindAsync(dto.Id);
                if (existingTask == null)
                {
                    return NotFound($"Task with Id {dto.Id} not found");
                }
                
                if (!string.IsNullOrWhiteSpace(dto.Descricao))
                    existingTask.Descricao = dto.Descricao;
                else if (!string.IsNullOrWhiteSpace(dto.Titulo))
                    existingTask.Descricao = dto.Titulo;
                    
                if (!string.IsNullOrWhiteSpace(dto.Status))
                    existingTask.Status = dto.Status;
                    
                if (dto.CriadaEm != null)
                {
                    existingTask.CriadaEm = ToUtc(dto.CriadaEm);
                }

                Console.WriteLine($"Status: {dto.Status}");

                if (dto.Status == "Concluído")
                {
                    Console.WriteLine("Concluído");
                    existingTask.Concluida = true;
                    existingTask.ConcluidaEm = ToUtc(dto.CriadaEm);
                }
                else
                {
                    existingTask.Concluida = false;
                }
                
                _context.Tarefas.Update(existingTask);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"Task updated - Id: {existingTask.Id}");
                return Ok(existingTask);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Parse Error: {ex.Message}");
                return BadRequest($"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<Tarefa>> DeleteTask(Tarefa task)
        {
            if (task == null)
            {
                return BadRequest("Task is null");
            }else{
                _context.Tarefas.Remove(task);
            }
            await _context.SaveChangesAsync(); 
            return Ok("Task deleted successfully");
        }
    }
}