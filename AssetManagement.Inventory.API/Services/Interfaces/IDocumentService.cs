using AssetManagement.Inventory.API.Domain.Enums;
using AssetManagement.Inventory.API.DTOs.DocumentDto;

namespace AssetManagement.Inventory.API.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<DocumentResponseDto> UploadAsync(UploadDocumentDto dto);
        Task<IEnumerable<DocumentResponseDto>> GetAllAsync();
        Task<IEnumerable<DocumentResponseDto>> GetByTypeAsync(DocumentType type);
        Task<DocumentDetailsDto?> GetByIdAsync(Guid id);
        Task<(byte[] FileBytes, string ContentType, string FileName)> DownloadAsync(Guid id);
    }
}
