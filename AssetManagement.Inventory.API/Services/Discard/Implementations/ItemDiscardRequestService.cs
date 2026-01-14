using AssetManagement.Inventory.API.Domain.Entities;
using AssetManagement.Inventory.API.Domain.Entities.Identity;
using AssetManagement.Inventory.API.Domain.Enums;
using AssetManagement.Inventory.API.DTOs.Item;
using AssetManagement.Inventory.API.Infrastructure.Data;
using AssetManagement.Inventory.API.Messaging.Constants;
using AssetManagement.Inventory.API.Messaging.Events;
using AssetManagement.Inventory.API.Messaging.RabbitMQ;
using AssetManagement.Inventory.API.Services.Discard.Interfaces;
using AssetManagement.Inventory.API.Messaging.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Inventory.API.Services.Discard.Implementations
{
    public class ItemDiscardRequestService : IItemDiscardRequestService
    {
        private readonly InventoryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;


        public ItemDiscardRequestService(
            InventoryDbContext context,
            UserManager<ApplicationUser> userManager, IRabbitMqPublisher rabbitMqPublisher)
        {
            _context = context;
            _userManager = userManager;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public async Task<DiscardRequestResponseDto> CreateAsync(CreateDiscardRequestDto dto, Guid userId)
        {
            var item = await _context.Items.Include(i => i.Area)
                                           .FirstOrDefaultAsync(i => i.Id == dto.ItemId);
            if (item == null) throw new Exception("Item não encontrado");
            if (item.Status == ItemStatus.Descartado)
                throw new Exception("Item já está descartado");

            var request = new ItemDiscardRequest
            {
                Id = Guid.NewGuid(),
                ItemId = item.Id,
                RequestedByUserId = userId,
                Justification = dto.Justification
            };

            _context.ItemDiscardRequests.Add(request);
            await _context.SaveChangesAsync();


            var user = await _userManager.FindByIdAsync(userId.ToString());


            _rabbitMqPublisher.Publish(
                new ItemDiscardRequestedEvent
                {
                    DiscardRequestId = request.Id,
                    ItemId = item.Id,
                    ItemName = item.Name,
                    AreaName = item.Area.Name,
                    RequestedBy = user?.UserName ?? "Desconhecido",
                    Justification = dto.Justification
                },
                RabbitQueues.ItemDiscardRequested
            );



            return new DiscardRequestResponseDto
            {
                Id = request.Id,
                ItemName = item.Name,
                AreaName = item.Area.Name,
                RequestedByName = (await _userManager.FindByIdAsync(userId.ToString()))?.UserName ?? "Desconhecido",
                Justification = request.Justification,
                Approved = request.Approved,
                CreatedAt = request.CreatedAt
            };
        }

        public async Task<IEnumerable<DiscardRequestResponseDto>> GetPendingAsync()
        {
            return await _context.ItemDiscardRequests
                .Where(r => !r.Approved)
                .Include(r => r.Item)
                    .ThenInclude(i => i.Area)
                .Include(r => r.RequestedBy)
                .Select(r => new DiscardRequestResponseDto
                {
                    Id = r.Id,
                    ItemName = r.Item.Name,
                    AreaName = r.Item.Area.Name,
                    RequestedByName = r.RequestedBy != null
                        ? r.RequestedBy.UserName
                        : "Usuário removido",
                    Justification = r.Justification,
                    Approved = r.Approved,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }


        public async Task ApproveAsync(Guid requestId, Guid adminId)
        {
            var request = await _context.ItemDiscardRequests
                .Include(r => r.Item)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) throw new Exception("Solicitação não encontrada");
            if (request.Approved) return;

            request.Approved = true;
            request.UpdatedAt = DateTime.UtcNow;

            request.Item.Status = ItemStatus.Descartado;
            request.Item.UpdatedAt = DateTime.UtcNow;

            _context.ItemDiscardRequests.Update(request);
            _context.Items.Update(request.Item);
            await _context.SaveChangesAsync();
        }
    }

}
