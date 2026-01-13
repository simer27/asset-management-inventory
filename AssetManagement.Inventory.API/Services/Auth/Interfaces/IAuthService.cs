using AssetManagement.Inventory.API.Domain.Enums;
using AssetManagement.Inventory.API.DTOs.Auth;

namespace AssetManagement.Inventory.API.Services.Auth.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
        Task LogoutAsync(string refreshToken);
        Task ConfirmEmailAsync(string userId, string token);
        Task ResendConfirmationEmailAsync(string email);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
        Task RevokeAllTokensAsync(Guid userId);
        Task<IEnumerable<UserResponseDto>> GetUsersAsync();
        Task UpdateUserRoleAsync(Guid userId, string role);

    }
}
