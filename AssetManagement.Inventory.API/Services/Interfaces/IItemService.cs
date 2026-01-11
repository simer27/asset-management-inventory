using AssetManagement.Inventory.API.DTOs.Item;

namespace AssetManagement.Inventory.API.Services.Interfaces
{
    public interface IItemService
    {
        Task<ItemResponseDto> CreateAsync(CreateItemDto dto);
        Task<byte[]> ExportExcelAsync();
        Task<byte[]> ExportPdfAsync();
        Task<IEnumerable<ItemResponseDto>> GetAllAsync();
        Task<ItemResponseDto?> GetByIdAsync(Guid id);        
        Task<string> UploadNotaFiscalAsync(Guid itemId, IFormFile file);
        Task<ItemResponseDto> UpdateAsync(Guid id, UpdateItemDto dto);
        Task<(byte[] FileBytes, string FileName, string ContentType)> DownloadNotaFiscalAsync(Guid itemId);
        Task DeleteAsync(Guid id);
        Task DeleteNotaFiscalAsync(Guid id);
    }
}
