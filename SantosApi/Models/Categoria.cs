using System.Text.Json.Serialization;

namespace SantosApi.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty; // Ex: Mártires
        public string CorHex { get; set; } = "#FFF0D4";  // A cor pastel do banner

        // Uma categoria tem vários santos
        [JsonIgnore] 
        public List<Santo> Santos { get; set; } = new List<Santo>();
    }
}