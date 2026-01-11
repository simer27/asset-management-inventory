using AssetManagement.Inventory.API.Domain.Enums;

namespace AssetManagement.Inventory.API.Domain.Entities
{
    public class Item
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public Guid AreaId { get; set; }
        public Area Area { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal? ValorMedio { get; set; }

        public string? NotaFiscalCaminho { get; set; }

        public ItemStatus Status { get; set; } = ItemStatus.Ativo;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
