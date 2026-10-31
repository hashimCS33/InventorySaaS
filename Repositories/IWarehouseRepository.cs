using System;
using InventorySaaS.Models;

namespace InventorySaaS.Repositories;

public interface IWarehouseRepository
{
    Task<IEnumerable<Warehouse>> GetAllWarehousesAsync(int tenantId);
    Task<Warehouse?> GetWarehouseByIdAsync(int tenantId, int id);
  
    Task AddWarehouseAsync(Warehouse warehouse);

    Task<bool> UpdateWarehouseAsync(int tenantId, Warehouse warehouse);

    Task<bool> DeleteWarehouseAsync(int tenantId, int id);
}
