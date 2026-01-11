using AssetManagement.Inventory.API.Domain.Enums;

namespace AssetManagement.Inventory.API.DTOs.Item
{
    public class CreateItemDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }   
        public int Quantity { get; set; }
        public decimal? ValorMedio { get; set; }

        public string? NotaFiscalCaminho { get; set; }

        public ItemStatus Status { get; set; } = ItemStatus.Ativo;

        public Guid AreaId { get; set; }
    }
}
