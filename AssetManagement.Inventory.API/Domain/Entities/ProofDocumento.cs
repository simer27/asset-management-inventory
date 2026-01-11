using AssetManagement.Inventory.API.Domain.Enums;

namespace AssetManagement.Inventory.API.Domain.Entities
{
    public class ProofDocumento
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DocumentType Type { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
