using AssetManagement.Inventory.API.Domain.Entities.Identity;
using AssetManagement.Inventory.API.DTOs.Auth;
using AssetManagement.Inventory.API.Services.Auth.Interfaces;
using AssetManagement.Inventory.API.Services.Email.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AssetManagement.Inventory.API.Services.Auth.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
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

    }
}
