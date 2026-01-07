using AssetManagement.Inventory.API.DTOs.Auth;
using AssetManagement.Inventory.API.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Inventory.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(
            [FromQuery] string userId,
            [FromQuery] string token)
        {
            await _authService.ConfirmEmailAsync(userId, token);
            return Ok("Email confirmado com sucesso.");
        }



        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation(
        [FromBody] ResendConfirmationDto dto)
        {
            await _authService.ResendConfirmationEmailAsync(dto.Email);
            return Ok("E-mail de confirmação reenviado.");
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto);
            return Ok("E-mail de recuperação enviado.");
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok("Senha alterada com sucesso.");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
        {
            await _authService.LogoutAsync(dto.RefreshToken);
            return Ok("Logout realizado com sucesso.");
        }

        [HttpPost("admin/revoke-user-tokens/{userId}")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> RevokeUserTokens(Guid userId)
        {
            await _authService.RevokeAllTokensAsync(userId);
            return Ok("Todos os tokens do usuário foram revogados.");
        }

    }
}
