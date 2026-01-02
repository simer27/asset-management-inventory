using AssetManagement.Inventory.API.DTOs.Auth;

namespace AssetManagement.Inventory.API.Services.Auth.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);
    }
}
