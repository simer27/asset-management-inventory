namespace AssetManagement.Inventory.API.DTOs.Item
{
    public class ItemResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal? ValorMedio { get; set; }

        public string? NotaFiscalCaminho { get; set; }

        public Guid AreaId { get; set; }
        public string AreaName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
