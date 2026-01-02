using AssetManagement.Inventory.API.DTOs.Auth;

namespace AssetManagement.Inventory.API.Services.Auth.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
        Task LogoutAsync(string refreshToken);

    }
}
