using AssetManagement.Inventory.API.DTOs.Messaging;

namespace AssetManagement.Inventory.API.Services.Email.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
        Task SendTermResponsibilityToAdminsAsync(TermResponsibilityUploadedEvent message);
    }
}
