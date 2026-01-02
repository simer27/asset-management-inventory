using AssetManagement.Inventory.API.Domain.Entities.Identity;
using AssetManagement.Inventory.API.DTOs.Auth;
using AssetManagement.Inventory.API.Infrastructure.Data;
using AssetManagement.Inventory.API.Services.Auth.Interfaces;
using AssetManagement.Inventory.API.Services.Email.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AssetManagement.Inventory.API.Services.Auth.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly InventoryDbContext _context;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService, IConfiguration configuration, InventoryDbContext context)
        {
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
            _context = context;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception("Erro ao criar usuário");

            // 🔐 GERAR TOKEN DE CONFIRMAÇÃO
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // 🔗 GERAR LINK (frontend)
            var confirmationLink =
                $"https://localhost:4200/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            // 📧 ENVIAR EMAIL
            await _emailService.SendAsync(
                user.Email!,
                "Confirmação de cadastro",
                $@"
                <p>Olá,</p>
                <p>Confirme seu cadastro clicando no link abaixo:</p>
                <a href='{confirmationLink}'>Confirmar cadastro</a>
            "
            );
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new UnauthorizedAccessException("Invalid credentials");

            if (!user.EmailConfirmed)
                throw new UnauthorizedAccessException("Email not confirmed");

            var accessToken = await GenerateJwtAsync(user);

            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:ExpiresInMinutes"]!)
                )
            };
        }



        private async Task<string> GenerateJwtAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:ExpiresInMinutes"]!)
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            var storedToken = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r =>
                    r.Token == dto.RefreshToken &&
                    !r.IsRevoked &&
                    r.ExpiresAt > DateTime.UtcNow);

            if (storedToken == null)
                throw new UnauthorizedAccessException("Invalid or expired refresh token");

            // 🔥 Rotate token
            storedToken.IsRevoked = true;

            var newRefreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = storedToken.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            var newAccessToken = await GenerateJwtAsync(storedToken.User);

            return new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:ExpiresInMinutes"]!)
                )
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (token == null)
                return;

            token.IsRevoked = true;
            await _context.SaveChangesAsync();
        }

    }
}
