using AssetManagement.Inventory.API.Domain.Enums;

namespace AssetManagement.Inventory.API.DTOs.Item
{
    public class UpdateItemStatusDto
    {
        public ItemStatus Status { get; set; }
    }
}
