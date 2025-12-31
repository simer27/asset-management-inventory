using AssetManagement.Inventory.API.DTOs.Item;

namespace AssetManagement.Inventory.API.Services.Interfaces
{
    public interface IItemService
    {
        Task<ItemResponseDto> CreateAsync(CreateItemDto dto);
        Task<IEnumerable<ItemResponseDto>> GetAllAsync();
        Task<ItemResponseDto?> GetByIdAsync(Guid id);
    }
}
