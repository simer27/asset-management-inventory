using AssetManagement.Inventory.API.Domain.Enums;

namespace AssetManagement.Inventory.API.DTOs.DocumentDto
{
    public class DocumentDetailsDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public DocumentType Type { get; set; }
        public string FileUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
