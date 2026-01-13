using AssetManagement.Inventory.API.Domain.Enums;

namespace AssetManagement.Inventory.API.DTOs.Auth
{
    public class UpdateUserRoleDto
    {
        public string Role { get; set; } = null!;
    }
}
