using System;
using InventorySaaS.Data;
using InventorySaaS.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySaaS.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
     private readonly AppDbContext _context;

    public WarehouseRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync(int tenantId)
    {
        return await _context.Warehouses
            .Where(w => w.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task<Warehouse?> GetWarehouseByIdAsync(int tenantId, int id)
    {
        return await _context.Warehouses.AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id && w.TenantId == tenantId);
    }

    public async Task AddWarehouseAsync(Warehouse warehouse)
    {
        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateWarehouseAsync(int tenantId, Warehouse warehouse)
    {
        var existingWarehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == warehouse.Id && w.TenantId == tenantId);

        if (existingWarehouse == null)
        {
            return false;
        }

        existingWarehouse.Name = warehouse.Name;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteWarehouseAsync(int tenantId, int id)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == id && w.TenantId == tenantId);

        if (warehouse == null)
        {
            return false;
        }

        _context.Warehouses.Remove(warehouse);
        await _context.SaveChangesAsync();
        return true;
    }
}

