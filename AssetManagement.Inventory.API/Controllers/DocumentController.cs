using AssetManagement.Inventory.API.Domain.Entities;
using AssetManagement.Inventory.API.Domain.Enums;
using AssetManagement.Inventory.API.DTOs.DocumentDto;
using AssetManagement.Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Inventory.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/documents")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [Authorize(Roles = "Admin, Master")]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadDocumentDto dto)
        {
            var result = await _documentService.UploadAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _documentService.GetAllAsync());
        }

        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetByType(DocumentType type)
        {
            return Ok(await _documentService.GetByTypeAsync(type));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var doc = await _documentService.GetByIdAsync(id);

            if (doc == null)
                return NotFound();

            return Ok(doc);
        }

        [HttpGet("{id:guid}/download")]
        public async Task<IActionResult> Download(Guid id)
        {
            var result = await _documentService.DownloadAsync(id);

            return File(result.FileBytes, result.ContentType, result.FileName);
        }
    }
}
