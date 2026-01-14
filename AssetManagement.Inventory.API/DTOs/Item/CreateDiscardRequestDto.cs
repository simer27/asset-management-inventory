namespace AssetManagement.Inventory.API.DTOs.Item
{
    public class CreateDiscardRequestDto
    {
        public Guid ItemId { get; set; }
        public string Justification { get; set; } = string.Empty;
    }
}
