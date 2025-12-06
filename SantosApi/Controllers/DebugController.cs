using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace SantosApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public DebugController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // PROMOVER USUÁRIO PARA ADMIN
        // Pode enviar no body o email como string (ex: "meu@email.com").
        // Se não enviar, usa o email padrão abaixo.
        [HttpPost("make-admin")]
        public async Task<IActionResult> MakeAdmin([FromBody] string? email)
        {
            var targetEmail = string.IsNullOrWhiteSpace(email)
                ? "mateusbragasan@gmail.com"
                : email.Trim().Replace("\"", ""); // remove aspas acidentais

            var user = await _userManager.FindByEmailAsync(targetEmail);
            if (user == null)
                return NotFound($"Usuário '{targetEmail}' não encontrado.");

            // garante a role Admin exista
            var roleResult = await _userManager.AddToRoleAsync(user, "Admin"); // se já tiver, ignora
            if (!roleResult.Succeeded)
                return BadRequest(roleResult.Errors);

            return Ok($"Usuário {targetEmail} agora é Admin.");
        }

        // DEBUG: mostra se o request chegou com token válido e quais claims o token tem
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {       
            var authenticated = User?.Identity?.IsAuthenticated ?? false;
            var name = User?.Identity?.Name;

        // converte cada claim para object para que o tipo seja List<object>
        var claims = User?.Claims?
            .Select(c => (object)new { c.Type, c.Value })
            .ToList() ?? new List<object>();

        // também retorna os headers recebidos (útil para ver Authorization)
        var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

        return Ok(new
        {
            IsAuthenticated = authenticated,
            Name = name,
            Claims = claims,
            Headers = headers
        });
        }
    }
}

