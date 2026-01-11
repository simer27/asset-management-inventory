using AssetManagement.Inventory.API.Domain.Enums;

namespace AssetManagement.Inventory.API.DTOs.DocumentDto
{
    public class DocumentResponseDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public DocumentType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
