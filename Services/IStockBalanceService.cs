using System;
using InventorySaaS.DTOs.StockBalances;

namespace InventorySaaS.Services;

public interface IStockBalanceService
{
   Task<IEnumerable<StockBalanceDto>> GetAllBalancesAsync(
        int tenantId);

    Task<StockBalanceDto?> GetBalanceAsync(
        int tenantId,
        int productId,
        int warehouseId);
}
