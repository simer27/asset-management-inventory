using AssetManagement.Inventory.API.Domain.Enums;

namespace AssetManagement.Inventory.API.DTOs.Item
{
    public class UpdateItemDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public Guid AreaId { get; set; }
        public decimal? ValorMedio { get; set; }
        public ItemStatus Status { get; set; }
    }
}
