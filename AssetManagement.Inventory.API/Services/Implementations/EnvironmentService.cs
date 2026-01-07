using AssetManagement.Inventory.API.Domain.Entities;
using AssetManagement.Inventory.API.DTOs.EnvironmentDto;
using AssetManagement.Inventory.API.Exceptions;
using AssetManagement.Inventory.API.Infrastructure.Data;
using AssetManagement.Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Inventory.API.Services.Implementations
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly InventoryDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EnvironmentService(InventoryDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<EnvironmentEntity> CreateAsync(CreateEnvironmentDto dto)
        {
            var entity = new EnvironmentEntity
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao
            };

            _context.Environments.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<IEnumerable<EnvironmentEntity>> GetAllAsync()
        {
            return await _context.Environments
                .Include(e => e.Imagens)
                .OrderBy(e => e.Nome)
                .ToListAsync();
        }

        public async Task<EnvironmentDetailsDto> GetByIdAsync(Guid id)
        {
            var entity = await _context.Environments
                .Include(e => e.Imagens)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null)
                throw new AppException("Ambiente não encontrado.", 404);

            return new EnvironmentDetailsDto
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Descricao = entity.Descricao,
                Imagens = entity.Imagens.Select(i => i.FilePath).ToList()
            };
        }

        public async Task UpdateAsync(Guid id, CreateEnvironmentDto dto)
        {
            var entity = await _context.Environments.FindAsync(id);

            if (entity == null)
                throw new AppException("Ambiente não encontrado.", 404);

            entity.Nome = dto.Nome;
            entity.Descricao = dto.Descricao;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Environments
                .Include(e => e.Imagens)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null)
                throw new AppException("Ambiente não encontrado.", 404);

            // remover arquivos físicos
            foreach (var img in entity.Imagens)
            {
                var filePath = Path.Combine(_env.WebRootPath, img.FilePath.TrimStart('/'));
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            _context.Environments.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddImagesAsync(Guid environmentId, List<IFormFile> imagens)
        {
            var entity = await _context.Environments
                .Include(e => e.Imagens)
                .FirstOrDefaultAsync(e => e.Id == environmentId);

            if (entity == null)
                throw new AppException("Ambiente não encontrado.", 404);

            if (entity.Imagens.Count + imagens.Count > 5)
                throw new AppException("Um ambiente pode ter no máximo 5 imagens.", 400);

            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "ambientes");

            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            foreach (var img in imagens)
            {
                var uniqueName = $"{Guid.NewGuid()}_{img.FileName}";
                var filePath = Path.Combine(uploadDir, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }

                entity.Imagens.Add(new EnvironmentImage
                {
                    FilePath = $"/uploads/ambientes/{uniqueName}",
                    EnvironmentId = environmentId
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
