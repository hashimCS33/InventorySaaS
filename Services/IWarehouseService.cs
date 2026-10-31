using System;
using InventorySaaS.DTOs.Warehouses;

namespace InventorySaaS.Services;

public interface IWarehouseService
{
    Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync(int tenantId);
    Task<WarehouseDto?> GetWarehouseByIdAsync(int tenantId, int id);
    Task<WarehouseDto> CreateWarehouseAsync(int tenantId, CreateWarehouseDto createWarehouseDto);
    Task<bool> UpdateWarehouseAsync(int tenantId, int id, UpdateWarehouseDto updateWarehouseDto);
    Task<bool> DeleteWarehouseAsync(int tenantId, int id);
}
