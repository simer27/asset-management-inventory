using AssetManagement.Inventory.API.Domain.Entities;
using AssetManagement.Inventory.API.DTOs.EnvironmentDto;

namespace AssetManagement.Inventory.API.Services.Interfaces
{
    public interface IEnvironmentService
    {
        Task<EnvironmentEntity> CreateAsync(CreateEnvironmentDto dto);
        Task<IEnumerable<EnvironmentListDto>> GetAllAsync();

        Task<EnvironmentDetailsDto> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, CreateEnvironmentDto dto);
        Task DeleteAsync(Guid id);
        Task AddImagesAsync(Guid environmentId, List<IFormFile> imagens);
        Task RemoveImageAsync(Guid environmentId, Guid imageId);
        Task <List<EnvironmentImageDto>>GetImagesAsync(Guid environmentId);

    }
}
