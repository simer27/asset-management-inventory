using AssetManagement.Inventory.API.DTOs.Messaging;

namespace AssetManagement.Inventory.API.Services.Notification.Interface
{
    public interface INotificationService
    {
        Task<int> GetUnreadCountAsync(Guid userId);
        Task<IEnumerable<NotificationDto>> GetAllAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId);
    }

}
