using AssetManagement.Inventory.API.DTOs.Messaging;
using AssetManagement.Inventory.API.Infrastructure.Data;
using AssetManagement.Inventory.API.Services.Notification.Interface;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Inventory.API.Services.Notification.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly InventoryDbContext _context;

        public NotificationService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<IEnumerable<NotificationDto>> GetAllAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return;
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }

    }
}
