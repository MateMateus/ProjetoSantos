using System.Text.Json.Serialization;

namespace SantosApi.Models
{
    public class Santo
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;

        // NOVOS CAMPOS DE DATA
        public string Nascimento { get; set; } = string.Empty;
        public string Falecimento { get; set; } = string.Empty;
        public string Festa { get; set; } = string.Empty;

        public string Historia { get; set; } = string.Empty;
        public string FotoUrl { get; set; } = string.Empty;
        public string CorHex { get; set; } = "#FFFFFF"; 
        public bool EhFamoso { get; set; } = false;

        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        // Listas
        public List<Milagre> Milagres { get; set; } = new List<Milagre>();
        public List<Local> Locais { get; set; } = new List<Local>();
        public List<ImagemGaleria> Imagens { get; set; } = new List<ImagemGaleria>();
    }
}