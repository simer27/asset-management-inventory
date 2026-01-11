using AssetManagement.Inventory.API.Domain.Enums;

namespace AssetManagement.Inventory.API.DTOs.DocumentDto
{
    public class UploadDocumentDto
    {
        public DocumentType Type { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
