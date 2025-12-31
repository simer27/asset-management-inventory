using AssetManagement.Inventory.API.DTOs.Area;
using AssetManagement.Inventory.API.DTOs.Item;

namespace AssetManagement.Inventory.API.Services.Interfaces
{
    public interface IAreaService
    {
        Task<AreaResponseDto> CreateAsync(CreateAreaDto dto);
        Task<IEnumerable<AreaResponseDto>> GetAllAsync();
        Task<IEnumerable<ItemResponseDto>> GetItemsByAreaAsync(Guid areaId);

    }
}
