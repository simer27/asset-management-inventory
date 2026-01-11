using AssetManagement.Inventory.API.Domain.Entities;
using AssetManagement.Inventory.API.DTOs.Area;
using AssetManagement.Inventory.API.DTOs.Item;
using AssetManagement.Inventory.API.Infrastructure.Data;
using AssetManagement.Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace AssetManagement.Inventory.API.Services.Implementations
{
    public class AreaService : IAreaService 
    {
        private readonly InventoryDbContext _context;

        public AreaService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<AreaResponseDto> CreateAsync(CreateAreaDto dto)
        {
            var exists = await _context.Areas
                .AnyAsync(a => a.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                throw new Exception("Área já cadastrada.");

            var area = new Area
            {
                Id = Guid.NewGuid(),
                Name = dto.Name
            };

            _context.Areas.Add(area);
            await _context.SaveChangesAsync();

            return new AreaResponseDto
            {
                Id = area.Id,
                Name = area.Name,
                TotalItems = 0
            };
        }

        public async Task<IEnumerable<AreaResponseDto>> GetAllAsync()
        {
            return await _context.Areas
                .Select(a => new AreaResponseDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    TotalItems = a.Items.Count
                })
                .ToListAsync();
        }

        public async Task<AreaResponseDto?> GetByIdAsync(Guid id)
        {
            return await _context.Areas
                .Where(a => a.Id == id)
                .Select(a => new AreaResponseDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    TotalItems = a.Items.Count
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AreaResponseDto> UpdateAsync(Guid id, UpdateAreaDto dto)
        {
            var area = await _context.Areas
                .Include(a => a.Items)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (area == null)
                throw new Exception("Área não encontrada.");

            var exists = await _context.Areas
                .AnyAsync(a => a.Name.ToLower() == dto.Name.ToLower() && a.Id != id);

            if (exists)
                throw new Exception("Já existe uma área com este nome.");

            area.Name = dto.Name;

            await _context.SaveChangesAsync();

            return new AreaResponseDto
            {
                Id = area.Id,
                Name = area.Name,
                TotalItems = area.Items.Count
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var area = await _context.Areas
                .Include(a => a.Items)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (area == null)
                throw new Exception("Área não encontrada.");

            if (area.Items.Any())
                throw new Exception("Não é possível excluir uma área que possui itens.");

            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();
        }



        public async Task<IEnumerable<ItemResponseDto>> GetItemsByAreaAsync(Guid areaId)
        {
            var areaExists = await _context.Areas.AnyAsync(a => a.Id == areaId);

            if (!areaExists)
                throw new Exception("Área não encontrada.");

            return await _context.Items
                .Where(i => i.AreaId == areaId)
                .Include(i => i.Area)
                .Select(i => new ItemResponseDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Quantity = i.Quantity,
                    ValorMedio = i.ValorMedio,
                    NotaFiscalCaminho = i.NotaFiscalCaminho,
                    AreaId = i.AreaId,
                    AreaName = i.Area.Name,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .ToListAsync();
        }

    }
}

