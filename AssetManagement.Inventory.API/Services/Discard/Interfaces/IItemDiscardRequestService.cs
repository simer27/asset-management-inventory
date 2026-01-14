using AssetManagement.Inventory.API.DTOs.Item;

namespace AssetManagement.Inventory.API.Services.Discard.Interfaces
{
    public interface IItemDiscardRequestService
    {
        Task<DiscardRequestResponseDto> CreateAsync(CreateDiscardRequestDto dto, Guid userId);
        Task<IEnumerable<DiscardRequestResponseDto>> GetPendingAsync();
        Task ApproveAsync(Guid requestId, Guid adminId);
    }

}
