using AssetManagement.Inventory.API.DTOs.Area;
using AssetManagement.Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AssetManagement.Inventory.API.Controllers
{
    
    [Authorize]
    [ApiController]
    [Route("api/areas")]
    public class AreasController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreasController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [Authorize(Roles = "Admin, Master")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAreaDto dto)
        {
            try
            {
                var result = await _areaService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetAll), result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao criar área");

            }
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var areas = await _areaService.GetAllAsync();
            return Ok(areas);
        }

        
        [HttpGet("{areaId:guid}/items")]
        public async Task<IActionResult> GetItemsByArea(Guid areaId)
        {
            try
            {
                var items = await _areaService.GetItemsByAreaAsync(areaId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
