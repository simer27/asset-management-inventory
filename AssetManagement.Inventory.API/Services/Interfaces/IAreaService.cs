using AssetManagement.Inventory.API.Domain.DTOs.Area;
using AssetManagement.Inventory.API.DTOs.Area;

namespace AssetManagement.Inventory.API.Services.Interfaces
{
    public interface IAreaService
    {
        Task<AreaResponseDto> CreateAsync(CreateAreaDto dto);
        Task<IEnumerable<AreaResponseDto>> GetAllAsync();
    }
}
