namespace AssetManagement.Inventory.API.Services.Email.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
