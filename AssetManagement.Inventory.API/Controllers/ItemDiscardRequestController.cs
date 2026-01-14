using AssetManagement.Inventory.API.DTOs.Item;
using AssetManagement.Inventory.API.Services.Discard.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AssetManagement.Inventory.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/discard-requests")]
    public class ItemDiscardRequestController : ControllerBase
    {
        private readonly IItemDiscardRequestService _service;

        public ItemDiscardRequestController(IItemDiscardRequestService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDiscardRequestDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _service.CreateAsync(dto, userId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Master")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _service.GetPendingAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Master")]
        [HttpPost("{requestId}/approve")]
        public async Task<IActionResult> Approve(Guid requestId)
        {
            var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _service.ApproveAsync(requestId, adminId);
            return NoContent();
        }
    }

}
