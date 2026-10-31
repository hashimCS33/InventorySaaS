using InventorySaaS.DTOs.Warehouses;
using InventorySaaS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventorySaaS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetAllWarehouses(int tenantId)
        {
            try
            {
                var warehouses = await _warehouseService.GetAllWarehousesAsync(tenantId);
                return Ok(warehouses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<WarehouseDto>> GetWarehouseById(int tenantId, int id)
        {
            try
            {
                var warehouse = await _warehouseService.GetWarehouseByIdAsync(tenantId, id);
                if (warehouse is null)
                {
                    return NotFound();
                }
                return Ok(warehouse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public async Task<ActionResult<WarehouseDto>> CreateWarehouse(int tenantId, [FromBody] CreateWarehouseDto createWarehouseDto)
        {
            {
                try
                {
                    var warehouse = await _warehouseService
                        .CreateWarehouseAsync(tenantId, createWarehouseDto);

                    return CreatedAtAction(
                        nameof(GetWarehouseById),
                        new { tenantId, id = warehouse.Id },
                        warehouse);
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

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> UpdateWarehouse(int tenantId, int id, [FromBody] UpdateWarehouseDto updateWarehouseDto)
        {
            try
            {
                var updated = await _warehouseService.UpdateWarehouseAsync(tenantId, id, updateWarehouseDto);
                if (!updated)
                {
                    return NotFound();
                }
                return NoContent();
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

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> DeleteWarehouse(int tenantId, int id)
        {
            try
            {
                var deleted = await _warehouseService.DeleteWarehouseAsync(tenantId, id);
                if (!deleted)
                {
                    return NotFound();
                }
                return NoContent();
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
