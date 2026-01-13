namespace AssetManagement.Inventory.API.DTOs.Auth
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
