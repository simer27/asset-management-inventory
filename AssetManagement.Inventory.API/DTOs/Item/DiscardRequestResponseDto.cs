namespace AssetManagement.Inventory.API.DTOs.Item
{
    public class DiscardRequestResponseDto
    {
        public Guid Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;
        public string RequestedByName { get; set; } = string.Empty;
        public string Justification { get; set; } = string.Empty;
        public bool Approved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
