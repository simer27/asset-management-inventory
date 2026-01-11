using AssetManagement.Inventory.API.DTOs.Area;
using AssetManagement.Inventory.API.DTOs.Item;

namespace AssetManagement.Inventory.API.Services.Interfaces
{
    public interface IAreaService
    {
        Task<AreaResponseDto> CreateAsync(CreateAreaDto dto);
        Task<IEnumerable<AreaResponseDto>> GetAllAsync();
        Task<AreaResponseDto?> GetByIdAsync(Guid id);
        Task<AreaResponseDto> UpdateAsync(Guid id, UpdateAreaDto dto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<ItemResponseDto>> GetItemsByAreaAsync(Guid areaId);

    }
}
