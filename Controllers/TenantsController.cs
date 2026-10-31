using InventorySaaS.DTOs;
using InventorySaaS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventorySaaS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllTenants()
        {
            var tenants = await _tenantService.GetAllTenantsAsync();
            return Ok(tenants);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetTenantById(int id)
        {
            var tenant = await _tenantService.GetTenantByIdAsync(id);
            if (tenant == null) return NotFound();
            return Ok(tenant);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantDto dto)
        {
            try
            {
                var tenant = await _tenantService.CreateTenantAsync(dto);
                return CreatedAtAction(nameof(GetTenantById), new { id = tenant.id }, tenant);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            
        }
     
        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> UpdateTenant(int id, [FromBody] UpdateTenantDto dto)
        {
            try
            {
                var updated = await _tenantService.UpdateTenantAsync(id, dto);
                if (!updated) return NotFound(new {message = "Tenant not found"});
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id:int:min(1)}andname={name}")]
        public async Task<IActionResult> DeleteTenant(int id, string name)
        {
            try
            {
                var deleted = await _tenantService.DeleteTenantAsync(id, name);
                if (!deleted) return NotFound(new {message = "Tenant not found"});
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
    
}
