using AssetManagement.Inventory.API.Domain.Entities;
using AssetManagement.Inventory.API.DTOs.Item;
using AssetManagement.Inventory.API.Infrastructure.Data;
using AssetManagement.Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace AssetManagement.Inventory.API.Services.Implementations
{
    public class ItemService : IItemService
    {
        private readonly InventoryDbContext _context;

        public ItemService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<ItemResponseDto> CreateAsync(CreateItemDto dto)
        {
            // Regra de negócio: Área deve existir
            var area = await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == dto.AreaId);

            if (area == null)
                throw new Exception("Área informada não existe.");

            // Regra: não permitir item duplicado na mesma área
            var exists = await _context.Items.AnyAsync(i =>
                i.Name.ToLower() == dto.Name.ToLower() &&
                i.AreaId == dto.AreaId);

            if (exists)
                throw new Exception("Item já cadastrado nesta área.");

            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Quantity = dto.Quantity,
                AreaId = dto.AreaId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return new ItemResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Quantity = item.Quantity,
                AreaId = area.Id,
                AreaName = area.Name,
                CreatedAt = item.CreatedAt
            };
        }

        public async Task<IEnumerable<ItemResponseDto>> GetAllAsync()
        {
            return await _context.Items
                .Include(i => i.Area)
                .Select(i => new ItemResponseDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Quantity = i.Quantity,
                    AreaId = i.AreaId,
                    AreaName = i.Area.Name,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<ItemResponseDto?> GetByIdAsync(Guid id)
        {
            return await _context.Items
                .Include(i => i.Area)
                .Where(i => i.Id == id)
                .Select(i => new ItemResponseDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Quantity = i.Quantity,
                    AreaId = i.AreaId,
                    AreaName = i.Area.Name,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }
    }
}
