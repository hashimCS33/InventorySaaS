using System;
using InventorySaaS.Data;
using InventorySaaS.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySaaS.Repositories;

public class StockBalanceRepository : IStockBalanceRepository
{
    private readonly AppDbContext _context;

    public StockBalanceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StockBalance>> GetAllBalancesAsync(int tenantId)
    {
        return await _context.StockBalances
            .Where(sb => sb.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task<StockBalance?> GetBalanceAsync(
        int tenantId,
        int productId,
        int warehouseId)
    {
        return await _context.StockBalances.AsNoTracking()
            .FirstOrDefaultAsync(sb => sb.TenantId == tenantId &&
                                       sb.ProductId == productId &&
                                       sb.WarehouseId == warehouseId);
    }
}
