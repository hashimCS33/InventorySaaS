using InventorySaaS.DTOs.StockMovements;
using InventorySaaS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventorySaaS.Controllers
{
    [Route("api/tenants/{tenantId:int:min(1)}/stock-movements")]
    [ApiController]
    public class StockMovementsController : ControllerBase
    {
        private readonly IStockMovementService _stockMovementService;

        public StockMovementsController(IStockMovementService stockMovementService)
        {
            _stockMovementService = stockMovementService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMovements(int tenantId)
        {
            try
            {
                var movements = await _stockMovementService.GetAllMovementsAsync(tenantId);
                return Ok(movements);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetMovementById(int tenantId, int id)
        {
            try
            {
                var movement = await _stockMovementService.GetMovementByIdAsync(tenantId, id);
                if (movement is null)
                {
                    return NotFound("حركة المخزون غير موجودة.");
                }
                return Ok(movement);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovement(int tenantId, [FromBody] CreateStockMovementDto dto)
        {
            try
            {
                var movement = await _stockMovementService.CreateMovementAsync(tenantId, dto);

                return CreatedAtAction(
                    nameof(GetMovementById),
                    new { tenantId, id = movement.Id },
                    movement);
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new { message = exception.Message });
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }
  
     [HttpPost("transfer")]
        public async Task<IActionResult> TransferStock(int tenantId, [FromBody] TransferStockDto dto)
        {
            try
            {
                var transferId = await _stockMovementService.TransferStockAsync(tenantId, dto);

                return Ok(new { transferId,
                message = "تم نقل المخزون بنجاح." });
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(new { message = exception.Message });
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }

    }
}
