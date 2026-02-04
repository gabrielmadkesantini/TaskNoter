using System.Text.Json.Serialization;

namespace tarefasApi.Models
{
    public class TarefaDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("titulo")]
        public string? Titulo {get; set;}
        
        [JsonPropertyName("descricao")]
        public string? Descricao {get; set;}

        [JsonPropertyName("concluido")]
        public bool Concluido {get; set;}

        [JsonPropertyName("status")]
        public string? Status {get; set;}

        [JsonPropertyName("criadaEm")]
        public DateTime CriadaEm {get; set;}
    }
}
