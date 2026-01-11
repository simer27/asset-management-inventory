using AssetManagement.Inventory.API.Domain.Entities;
using AssetManagement.Inventory.API.Domain.Enums;
using AssetManagement.Inventory.API.DTOs.DocumentDto;
using AssetManagement.Inventory.API.Infrastructure.Data;
using AssetManagement.Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Inventory.API.Services.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly InventoryDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DocumentService(InventoryDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<DocumentResponseDto> UploadAsync(UploadDocumentDto dto)
        {
            var dir = Path.Combine(_env.WebRootPath, "uploads", "documents");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var filePath = Path.Combine(dir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await dto.File.CopyToAsync(stream);

            var document = new ProofDocumento
            {
                Id = Guid.NewGuid(),
                FileName = dto.File.FileName,
                FilePath = $"/uploads/documents/{fileName}",
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return new DocumentResponseDto
            {
                Id = document.Id,
                FileName = document.FileName,
                Type = document.Type,
                CreatedAt = document.CreatedAt
            };
        }

        public async Task<IEnumerable<DocumentResponseDto>> GetAllAsync()
        {
            return await _context.Documents
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new DocumentResponseDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    Type = d.Type,
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentResponseDto>> GetByTypeAsync(DocumentType type)
        {
            return await _context.Documents
                .Where(d => d.Type == type)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new DocumentResponseDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    Type = d.Type,
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<DocumentDetailsDto?> GetByIdAsync(Guid id)
        {
            return await _context.Documents
                .Where(d => d.Id == id)
                .Select(d => new DocumentDetailsDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    Type = d.Type,
                    FileUrl = d.FilePath,
                    CreatedAt = d.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(byte[] FileBytes, string ContentType, string FileName)> DownloadAsync(Guid id)
        {
            var document = await _context.Documents.FindAsync(id);

            if (document == null)
                throw new Exception("Documento não encontrado.");

            var filePath = Path.Combine(
                _env.WebRootPath,
                document.FilePath.TrimStart('/')
            );

            if (!File.Exists(filePath))
                throw new Exception("Arquivo não encontrado.");

            var bytes = await File.ReadAllBytesAsync(filePath);

            var contentType = Path.GetExtension(filePath).ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            return (bytes, contentType, document.FileName);
        }
    }
}
