namespace AssetManagement.Inventory.API.DTOs.Area
{
    public class AreaResponseDto
    {
        public Guid Id { get; set; }    
        public string Name { get; set; } = null!;
        public int TotalItems { get; set; }
    }
}
