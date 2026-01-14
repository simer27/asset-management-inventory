using AssetManagement.Inventory.API.Services.Email.Interfaces;
using AssetManagement.Inventory.API.Domain.Entities.Identity;
using AssetManagement.Inventory.API.DTOs.Messaging;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace AssetManagement.Inventory.API.Services.Email.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailService(
            IConfiguration config,
            UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        
        public async Task SendAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["Email:From"]!));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            email.Body = new BodyBuilder
            {
                HtmlBody = body
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _config["Email:Smtp"],
                int.Parse(_config["Email:Port"]!),
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _config["Email:User"],
                _config["Email:Password"]
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
                email.From.Add(MailboxAddress.Parse(_config["Email:From"]!));
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
                    _config["Email:Smtp"],
                    int.Parse(_config["Email:Port"]!),
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    _config["Email:User"],
                    _config["Email:Password"]
                );

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
