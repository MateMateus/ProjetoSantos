using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SantosApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // 1. CRIAR CONTA (Registrar)
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] LoginDto model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Senha);

            if (result.Succeeded)
            {
                return Ok(new { message = "Usuário criado com sucesso!" });
            }

            return BadRequest(result.Errors);
        }

        // 2. FAZER LOGIN (Gerar Token)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            // Verifica se usuário existe e se a senha bate
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Senha))
            {
                // Se deu certo, GERA O TOKEN (O Crachá)
                var tokenHandler = new JwtSecurityTokenHandler();
                // Pega a chave secreta do appsettings.json
                var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Key"] ?? "ChavePadraoMuitoLongaParaSeguranca123");
                
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email)
                    }),
                    Expires = DateTime.UtcNow.AddHours(8), // Token vale por 8 horas
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { Token = tokenString, User = user.Email });
            }

            return Unauthorized(new { message = "E-mail ou senha inválidos" });
        }
    }

    // Classe simples para receber os dados
    public class LoginDto
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}