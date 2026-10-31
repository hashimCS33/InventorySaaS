using InventorySaaS.DTOs.Products;
using InventorySaaS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventorySaaS.Controllers
{
  
    [ApiController]
    [Route("api/tenants/{tenantId:int:min(1)}/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProducts(int tenantId)
        {
            try
            {
                var products = await _service.GetAllProductsAsync(tenantId);
                return Ok(products);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
      
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> GetProductById(int id, int tenantId)
    {
        try
        {
            var product = await _service.GetProductByIdAsync(tenantId, id);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            return Ok(product);
        }

        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("create")]
   public async Task<ActionResult<ProductDto>> Create(
    int tenantId,
    [FromBody] CreateProductDto dto)
    {
        try
        {
            var product = await _service.CreateProductAsync(tenantId, dto);
            
            return CreatedAtAction(
                nameof(GetProductById),
                 new {
                     id = product.Id,
                      tenantId },
                       product);

        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int:min(1)}")]

    public async Task<IActionResult> Update(
        int tenantId,
        int id,
        [FromBody] UpdateProductDto dto)
    {
        try
        {
            var updated = await _service.UpdateProductAsync(tenantId, id, dto);
            if (!updated)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }


    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> DeleteProduct(int tenantId, int id)
    {
        try
        {
            var deleted = await _service.DeleteProductAsync(tenantId, id);
            if (!deleted)
            {
                return NotFound(new { message = $"Product with ID {id} not found." });
            }
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
    }
}
