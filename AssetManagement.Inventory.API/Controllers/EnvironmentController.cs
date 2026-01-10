using AssetManagement.Inventory.API.DTOs.EnvironmentDto;
using AssetManagement.Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnvironmentController : ControllerBase
    {
        private readonly IEnvironmentService _service;

        public EnvironmentController(IEnvironmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Criar novo ambiente (sem imagens ou com imagens)
        /// </summary>
        [Authorize(Roles = "Admin,Master")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateEnvironmentDto dto)
        {
            var env = await _service.CreateAsync(dto);

            if (dto.Images != null && dto.Images.Any())
                await _service.AddImagesAsync(env.Id, dto.Images);

            return CreatedAtAction(nameof(GetById), new { id = env.Id }, env);
        }

        /// <summary>
        /// Listar ambientes
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }


        /// <summary>
        /// Buscar ambiente por ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Atualizar dados do ambiente
        /// </summary>
        [Authorize(Roles = "Admin,Master")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] CreateEnvironmentDto dto)
        {
            await _service.UpdateAsync(id, dto);

            if (dto.Images != null && dto.Images.Any())
                await _service.AddImagesAsync(id, dto.Images);

            return NoContent();
        }

        /// <summary>
        /// Remover ambiente
        /// </summary>
        [Authorize(Roles = "Admin,Master")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Adicionar imagens em um ambiente (máx. 5)
        /// </summary>
        [Authorize(Roles = "Admin,Master")]
        [HttpPost("{id:guid}/images")]
        public async Task<IActionResult> AddImages(Guid id, [FromForm] List<IFormFile> imagens)
        {
            await _service.AddImagesAsync(id, imagens);
            return Ok(new { Message = "Imagens adicionadas com sucesso" });
        }


        /// <summary>
        /// Remover uma imagem específica de um ambiente
        /// </summary>
        [Authorize(Roles = "Admin,Master")]
        [HttpDelete("{environmentId:guid}/images/{imageId:guid}")]
        public async Task<IActionResult> RemoveImage(Guid environmentId, Guid imageId)
        {
            await _service.RemoveImageAsync(environmentId, imageId);
            return NoContent();
        }

        
        [HttpGet("{id:guid}/images")]
        public async Task<IActionResult> GetImages(Guid id)
        {
            var imagens = await _service.GetImagesAsync(id);
            return Ok(imagens);
        }


    }
}
