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

        public async Task<IEnumerable<EnvironmentListDto>> GetAllAsync()
        {
            return await _context.Environments
                .Include(e => e.Imagens)
                .OrderBy(e => e.Nome)
                .Select(e => new EnvironmentListDto
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    Descricao = e.Descricao,
                    Imagens = e.Imagens.Select(i => i.FilePath).ToList()
                })
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
            Console.WriteLine("🔍 Iniciando upload de imagens...");

            if (imagens == null || !imagens.Any())
                throw new AppException("Nenhuma imagem enviada.", 400);

            var entity = await _context.Environments
                .Include(e => e.Imagens)
                .FirstOrDefaultAsync(e => e.Id == environmentId);

            if (entity == null)
                throw new AppException("Ambiente não encontrado.", 404);

            if (entity.Imagens.Count + imagens.Count > 5)
                throw new AppException("Um ambiente pode ter no máximo 5 imagens.", 400);

            // Verificar WebRootPath
            Console.WriteLine($"WebRootPath: {_env.WebRootPath}");

            if (string.IsNullOrEmpty(_env.WebRootPath))
                throw new Exception("WebRootPath está NULL. Configure UseStaticFiles() no Program.cs.");

            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "ambientes");

            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            foreach (var img in imagens)
            {
                Console.WriteLine($"➡️ Salvando imagem: {img.FileName}");

                var extension = Path.GetExtension(img.FileName);
                var uniqueName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadDir, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }

                entity.Imagens.Add(new EnvironmentImage
                {
                    FileName = uniqueName,
                    FilePath = $"/uploads/ambientes/{uniqueName}",
                    EnvironmentId = environmentId
                });

                Console.WriteLine("✔️ Imagem salva no banco.");
            }


            await _context.SaveChangesAsync();
            Console.WriteLine("🎉 Upload finalizado com sucesso!");
        }


        public async Task RemoveImageAsync(Guid environmentId, Guid imageId)
        {
            var environment = await _context.Environments
                .Include(e => e.Imagens)
                .FirstOrDefaultAsync(e => e.Id == environmentId);

            if (environment == null)
                throw new AppException("Ambiente não encontrado.", 404);

            var image = environment.Imagens.FirstOrDefault(i => i.Id == imageId);

            if (image == null)
                throw new AppException("Imagem não encontrada.", 404);

            // remover arquivo físico
            var filePath = Path.Combine(_env.WebRootPath, image.FilePath.TrimStart('/'));
            if (File.Exists(filePath))
                File.Delete(filePath);

            _context.EnvironmentImages.Remove(image);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EnvironmentImageDto>> GetImagesAsync(Guid environmentId)
        {
            var entity = await _context.Environments
                .Include(e => e.Imagens)
                .FirstOrDefaultAsync(e => e.Id == environmentId);

            if (entity == null)
                throw new AppException("Ambiente não encontrado.", 404);

            return entity.Imagens
                .Select(img => new EnvironmentImageDto
                {
                    Id = img.Id,
                    FileName = img.FileName,
                    FilePath = img.FilePath
                })
                .ToList();
        }


    }
}
