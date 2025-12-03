using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SantosApi.Data;
using SantosApi.Models;

namespace SantosApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SantosController : ControllerBase
    {
        private readonly AppDbContext _context;

        private const string IMAGEM_PADRAO = "https://via.placeholder.com/300?text=Santo";

        public SantosController(AppDbContext context)
        {
            _context = context;
        }

        // --- GET ALL ---
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Santo>>> GetSantos()
        {
            return await _context.Santos
                .Include(s => s.Categoria)
                .ToListAsync();
        }

        // --- GET POR ID ---
        [HttpGet("{id}")]
        public async Task<ActionResult<Santo>> GetSanto(int id)
        {
            var santo = await _context.Santos
                .Include(s => s.Categoria)
                .Include(s => s.Milagres)
                .Include(s => s.Locais)
                .Include(s => s.Imagens)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (santo == null) return NotFound();
            return santo;
        }

        // --- GET CATEGORIA ---
        [HttpGet("categoria/{categoriaId}")]
        public async Task<ActionResult<IEnumerable<Santo>>> GetPorCategoria(int categoriaId)
        {
            return await _context.Santos
                .Where(s => s.CategoriaId == categoriaId)
                .Include(s => s.Categoria)
                .ToListAsync();
        }

        // --- GET BUSCA ---
        [HttpGet("busca/{termo}")]
        public async Task<ActionResult<IEnumerable<Santo>>> BuscarSanto(string termo)
        {
            return await _context.Santos
                .Where(s => s.Nome.Contains(termo) || s.Historia.Contains(termo))
                .Include(s => s.Categoria)
                .ToListAsync();
        }

        // --- POST EM LOTE ---
        [HttpPost("lote")]
        public async Task<IActionResult> PostLote([FromBody] List<Santo> santos)
        {
            if (santos == null || !santos.Any())
                return BadRequest("A lista enviada está vazia.");

            foreach (var s in santos)
            {
                // Categoria padrão
                if (s.CategoriaId == 0)
                    s.CategoriaId = 11;

                // Foto padrão
                if (string.IsNullOrWhiteSpace(s.FotoUrl))
                    s.FotoUrl = IMAGEM_PADRAO;

                // Garantir listas
                if (s.Imagens == null)
                    s.Imagens = new List<ImagemGaleria>();

                if (s.Milagres == null)
                    s.Milagres = new List<Milagre>();

                if (s.Locais == null)
                    s.Locais = new List<Local>();

                // Pelo menos 3 imagens extras
                while (s.Imagens.Count < 3)
                    s.Imagens.Add(new ImagemGaleria { Url = IMAGEM_PADRAO });
            }

            _context.Santos.AddRange(santos);
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = $"{santos.Count} santos foram cadastrados com sucesso!" });
        }

        // --- POST INDIVIDUAL ---
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Santo>> PostSanto(Santo santo)
        {
            if (santo.CategoriaId == 0)
                santo.CategoriaId = 11;

            if (string.IsNullOrWhiteSpace(santo.FotoUrl))
                santo.FotoUrl = IMAGEM_PADRAO;

            if (santo.Imagens == null)
                santo.Imagens = new List<ImagemGaleria>();

            while (santo.Imagens.Count < 3)
                santo.Imagens.Add(new ImagemGaleria { Url = IMAGEM_PADRAO });

            if (santo.Milagres == null)
                santo.Milagres = new List<Milagre>();

            if (santo.Locais == null)
                santo.Locais = new List<Local>();

            _context.Santos.Add(santo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSanto", new { id = santo.Id }, santo);
        }

        // --- PUT ---
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSanto(int id, Santo santoAtualizado)
        {
            if (id != santoAtualizado.Id)
                return BadRequest();

            var santoExistente = await _context.Santos
                .Include(s => s.Milagres)
                .Include(s => s.Locais)
                .Include(s => s.Imagens)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (santoExistente == null)
                return NotFound();

            // Atualiza dados principais
            santoExistente.Nome = santoAtualizado.Nome;
            santoExistente.Titulo = santoAtualizado.Titulo;
            santoExistente.Nascimento = santoAtualizado.Nascimento;
            santoExistente.Falecimento = santoAtualizado.Falecimento;
            santoExistente.Festa = santoAtualizado.Festa;
            santoExistente.Historia = santoAtualizado.Historia;
            santoExistente.CorHex = santoAtualizado.CorHex;
            santoExistente.EhFamoso = santoAtualizado.EhFamoso;
            santoExistente.CategoriaId = santoAtualizado.CategoriaId;

            santoExistente.FotoUrl = string.IsNullOrWhiteSpace(santoAtualizado.FotoUrl)
                ? IMAGEM_PADRAO
                : santoAtualizado.FotoUrl;

            // LIMPAR ANTIGAS LISTAS
            _context.Milagres.RemoveRange(santoExistente.Milagres);
            _context.Locais.RemoveRange(santoExistente.Locais);
            _context.ImagensGaleria.RemoveRange(santoExistente.Imagens);

            // ADICIONAR NOVAS
            santoExistente.Milagres = santoAtualizado.Milagres ?? new List<Milagre>();
            santoExistente.Locais = santoAtualizado.Locais ?? new List<Local>();
            santoExistente.Imagens = santoAtualizado.Imagens ?? new List<ImagemGaleria>();

            while (santoExistente.Imagens.Count < 3)
                santoExistente.Imagens.Add(new ImagemGaleria { Url = IMAGEM_PADRAO });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // --- DELETE ---
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSanto(int id)
        {
            var santo = await _context.Santos.FindAsync(id);

            if (santo == null)
                return NotFound();

            _context.Santos.Remove(santo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
