using System.Text.Json.Serialization;

namespace SantosApi.Models
{
    public class Local
    {
        public int Id { get; set; }
        public string NomeLugar { get; set; } = string.Empty; // Ex: Assis, Itália
        public string Descricao { get; set; } = string.Empty; // O que ele fez lá

        public int SantoId { get; set; }
        [JsonIgnore]
        public Santo? Santo { get; set; }
    }
}