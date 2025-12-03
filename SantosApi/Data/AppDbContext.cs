using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SantosApi.Models;

namespace SantosApi.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Santo> Santos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Milagre> Milagres { get; set; }
        public DbSet<Local> Locais { get; set; }
        public DbSet<ImagemGaleria> ImagensGaleria { get; set; } // <--- NOVO!

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nome = "Mártires", CorHex = "#FFB7B2" },
                new Categoria { Id = 2, Nome = "Marianos e Virgens", CorHex = "#AEC6CF" },
                new Categoria { Id = 3, Nome = "Monges e Franciscanos", CorHex = "#C8B4A2" },
                new Categoria { Id = 4, Nome = "Doutores e Papas", CorHex = "#FDFD96" },
                new Categoria { Id = 5, Nome = "Apóstolos", CorHex = "#B0E57C" },
                new Categoria { Id = 6, Nome = "Confessores", CorHex = "#C3B1E1" },
                new Categoria { Id = 7, Nome = "Santos da Caridade", CorHex = "#FFDAC1" },
                new Categoria { Id = 8, Nome = "Santos Jovens", CorHex = "#FFB7CE" },
                new Categoria { Id = 9, Nome = "Fundadores", CorHex = "#B2F0E8" },
                new Categoria { Id = 10, Nome = "Místicos", CorHex = "#D3D3D3" },
                new Categoria { Id = 11, Nome = "Geral", CorHex = "#FFF0D4" },
                new Categoria { Id = 12, Nome = "Anjos e Arcanjos", CorHex = "#C1E1C1" }
            );
        }
    }
}