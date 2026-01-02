using AssetManagement.Inventory.API.DTOs.Auth;
using AssetManagement.Inventory.API.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Inventory.API.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            await _authService.RegisterAsync(dto);
            return Ok("Cadastro realizado. Verifique seu e-mail.");
        }

    }
}
