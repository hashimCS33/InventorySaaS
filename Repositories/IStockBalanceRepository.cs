using System;
using InventorySaaS.Models;

namespace InventorySaaS.Repositories;

public interface IStockBalanceRepository
{
    Task<IEnumerable<StockBalance>> GetAllBalancesAsync(int tenantId);

    Task<StockBalance?> GetBalanceAsync(
        int tenantId,
        int productId,
        int warehouseId);

}
