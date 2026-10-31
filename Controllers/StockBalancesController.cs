using InventorySaaS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventorySaaS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockBalancesController : ControllerBase
    {
        private readonly IStockBalanceService _stockBalanceService;

        public StockBalancesController(IStockBalanceService stockBalanceService)
        {
            _stockBalanceService = stockBalanceService;
        }

        [HttpGet("{tenantId}")]
        public async Task<IActionResult> GetAllBalances(int tenantId)
        {
            try
            {
                var balances = await _stockBalanceService.GetAllBalancesAsync(tenantId);
                return Ok(balances);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{tenantId}/{productId}/{warehouseId}")]
        public async Task<IActionResult> GetBalance(int tenantId, int productId, int warehouseId)
        {
            try
            {
                var balance = await _stockBalanceService.GetBalanceAsync(tenantId, productId, warehouseId);
                if (balance is null)
                {
                    return NotFound("رصيد المخزون غير موجود.");
                }
                return Ok(balance);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
