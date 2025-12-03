using System.Text.Json.Serialization;

namespace SantosApi.Models
{
    public class Milagre
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty; // Ex: Cura da cegueira
        public string Descricao { get; set; } = string.Empty;

        public int SantoId { get; set; }
        [JsonIgnore]
        public Santo? Santo { get; set; }
    }
}