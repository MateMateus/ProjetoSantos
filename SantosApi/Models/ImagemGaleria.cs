using System.Text.Json.Serialization;

namespace SantosApi.Models
{
    public class ImagemGaleria
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;

        public int SantoId { get; set; }
        [JsonIgnore]
        public Santo? Santo { get; set; }
    }
}