using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

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

        [HttpPost("make-admin")]
        public async Task<IActionResult> MakeAdmin()
        {
            var user = await _userManager.FindByEmailAsync("mateusbragasan@gmail.com");

            if (user == null)
                return NotFound("Usuário não encontrado.");

            var result = await _userManager.AddToRoleAsync(user, "Admin");

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Usuário agora é Admin!");
        }
    }
}
