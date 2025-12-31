namespace AssetManagement.Inventory.API.DTOs.Item
{
    public class CreateItemDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }   
        public int Quantity { get; set; }

        public Guid AreaId { get; set; }
    }
}
