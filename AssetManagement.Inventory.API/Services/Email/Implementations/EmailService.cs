using AssetManagement.Inventory.API.Services.Email.Interfaces;
using System.Net.Mail;
using System.Net;

namespace AssetManagement.Inventory.API.Services.Email.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            using var smtp = new SmtpClient(_config["Email:Smtp"], int.Parse(_config["Email:Port"]!))
            {
                Credentials = new NetworkCredential(
                    _config["Email:User"],
                    _config["Email:Password"]
                ),
                EnableSsl = true
            };

            var message = new MailMessage(
                _config["Email:From"],
                to,
                subject,
                body
            )
            {
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}
