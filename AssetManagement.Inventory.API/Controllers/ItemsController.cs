using AssetManagement.Inventory.API.DTOs.EnvironmentDto;
using AssetManagement.Inventory.API.DTOs.Item;
using AssetManagement.Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Inventory.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [Authorize(Roles = "Admin, Master")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateItemDto dto)
        {
            try
            {
                var result = await _itemService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao criar item");
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _itemService.GetAllAsync();
            return Ok(items);
        }

       
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _itemService.GetByIdAsync(id);

            if (item == null)
                return NotFound(new { message = "Item não encontrado." });

            return Ok(item);
        }

        [Authorize(Roles = "Admin, Master")]
        [HttpPost("{id:guid}/nota-fiscal")]
        public async Task<IActionResult> UploadNotaFiscal(Guid id, [FromForm] UploadNotaFiscalDto dto)
        {
            var result = await _itemService.UploadNotaFiscalAsync(id, dto.NotaFiscal);
            return Ok(result);
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportExcel()
        {
            var fileBytes = await _itemService.ExportExcelAsync();

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");

            string fileName = $"Inventario_ADBras_Curuca_{timestamp}.xlsx";

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }



        //[HttpGet("export/pdf")]
        //public async Task<IActionResult> ExportPdf()
        //{
        //    var bytes = await _itemService.ExportPdfAsync();

        //    var fileName = $"Inventario_ADBras_Curuca_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

        //    return File(bytes, "application/pdf", fileName);
        //}


    }
}
