using System;
using InventorySaaS.DTOs.StockBalances;
using InventorySaaS.Models;
using InventorySaaS.Repositories;

namespace InventorySaaS.Services;

public class StockBalanceService : IStockBalanceService
{
    private readonly IStockBalanceRepository _stockBalanceRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IProductRepository _productRepository;

    private readonly IWarehouseRepository _warehouseService;

    public StockBalanceService(
        IStockBalanceRepository stockBalanceRepository,
        ITenantRepository tenantRepository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseService)
    {
        _stockBalanceRepository = stockBalanceRepository;
        _tenantRepository = tenantRepository;
        _productRepository = productRepository;
        _warehouseService = warehouseService;
    }

    public async Task<IEnumerable<StockBalanceDto>> GetAllBalancesAsync(int tenantId)
    {
        await ValidateTenantAsync(tenantId);
        var balances = await _stockBalanceRepository.GetAllBalancesAsync(tenantId);
        return balances.Select(ToDto).ToList();
    }

    public async Task<StockBalanceDto?> GetBalanceAsync(int tenantId, int productId, int warehouseId)
    {
         
         await ValidateTenantAsync(tenantId);

        var product = await _productRepository.GetProductByIdAsync(tenantId, productId);
        
        if (product is null)
        {
            throw new KeyNotFoundException("المنتج غير موجود.");
        }

        var warehouse = await _warehouseService.GetWarehouseByIdAsync(tenantId, warehouseId);
        if (warehouse is null)
        {
            throw new KeyNotFoundException("المستودع غير موجود.");
        }

        var balance = await _stockBalanceRepository.GetBalanceAsync(tenantId, productId, warehouseId);
        if (balance is null)
        {
            return null;
        }
        return ToDto(balance);
    }
    
     private async Task ValidateTenantAsync(int tenantId)
    {
        var tenant = await _tenantRepository
            .GetTenantByIdAsync(tenantId);

        if (tenant is null)
        {
            throw new KeyNotFoundException("الشركة غير موجودة.");
        }
    }

    private static StockBalanceDto ToDto(StockBalance balance)
    {
        return new StockBalanceDto
        {
            Id = balance.Id,
            TenantId = balance.TenantId,
            ProductId = balance.ProductId,
            WarehouseId = balance.WarehouseId,
            Quantity = balance.Quantity
        };
    }
}
