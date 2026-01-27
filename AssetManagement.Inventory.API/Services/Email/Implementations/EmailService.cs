using AssetManagement.Inventory.API.Services.Email.Interfaces;
using AssetManagement.Inventory.API.Domain.Entities.Identity;
using AssetManagement.Inventory.API.DTOs.Messaging;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using AssetManagement.Inventory.API.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace AssetManagement.Inventory.API.Services.Email.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailService(
        IOptions<EmailSettings> options,
        UserManager<ApplicationUser> userManager)
            {
                _settings = options.Value;
                _userManager = userManager;

            if (string.IsNullOrWhiteSpace(_settings.Smtp))
                throw new InvalidOperationException(
                    "Configuração de Email não encontrada. Verifique o .env"
                );
        }



        public async Task SendAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_settings.From!));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            email.Body = new BodyBuilder
            {
                HtmlBody = body
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _settings.Smtp,
                _settings.Port,
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _settings.User,
                _settings.Password
            );

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }



        public async Task SendTermResponsibilityToAdminsAsync(
            TermResponsibilityUploadedEvent message)
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            foreach (var admin in admins)
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_settings.From!));

                email.To.Add(MailboxAddress.Parse(admin.Email!));
                email.Subject = "Novo Termo de Responsabilidade";

                var builder = new BodyBuilder
                {
                    TextBody = "Um novo termo de responsabilidade foi enviado."
                };

                var physicalPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    message.FilePath.TrimStart('/')
                );

                builder.Attachments.Add(physicalPath);

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(
                    _settings.Smtp,
                    _settings.Port,
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    _settings.User,
                    _settings.Password
                );


                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
