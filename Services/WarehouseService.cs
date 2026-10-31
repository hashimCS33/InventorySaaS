using System;
using InventorySaaS.DTOs.Warehouses;
using InventorySaaS.Models;
using InventorySaaS.Repositories;

namespace InventorySaaS.Services;

public class WarehouseService : IWarehouseService
{
     private readonly IWarehouseRepository _warehouseRepository;
     private readonly ITenantRepository _tenantRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository, ITenantRepository tenantRepository)
    {
        _warehouseRepository = warehouseRepository;
        _tenantRepository = tenantRepository;
    }

    public async Task<WarehouseDto> CreateWarehouseAsync(int tenantId, CreateWarehouseDto createWarehouseDto)
    {
        await ValidateTenantAsync(tenantId, requireActive: true);

        var warehouse = new Warehouse
        {
            Name = ValidateName(createWarehouseDto.Name),
            TenantId = tenantId
        };

        await _warehouseRepository.AddWarehouseAsync(warehouse);
        return ToDto(warehouse);
    }

    public async Task<bool> DeleteWarehouseAsync(int tenantId, int id)
    {
        await ValidateTenantAsync(tenantId, requireActive: true);

        return await _warehouseRepository.DeleteWarehouseAsync(tenantId, id);
    }

    public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync(int tenantId)
    {
         await ValidateTenantAsync(tenantId, requireActive: true);

        var warehouse = await _warehouseRepository
        .GetAllWarehousesAsync(tenantId);
        

        return warehouse.Select(ToDto).ToList();

    }

    public async Task<WarehouseDto?> GetWarehouseByIdAsync(int tenantId, int id)
    {
        await ValidateTenantAsync(tenantId, requireActive: true);

        var warehouse = await _warehouseRepository.GetWarehouseByIdAsync(tenantId, id);
        return warehouse is null ? null : ToDto(warehouse);
    }

    public async Task<bool> UpdateWarehouseAsync(int tenantId, int id, UpdateWarehouseDto updateWarehouseDto)
    {
        await ValidateTenantAsync(tenantId, requireActive: true);

        var warehouse = await _warehouseRepository.GetWarehouseByIdAsync(tenantId, id);
        if (warehouse is null)
        {
           return false;
        }

        warehouse.Name = ValidateName(updateWarehouseDto.Name);

       

        return await _warehouseRepository.UpdateWarehouseAsync(tenantId, warehouse);
    }

     private async Task ValidateTenantAsync(
        int tenantId,
        bool requireActive = false)
    {
        var tenant = await _tenantRepository
            .GetTenantByIdAsync(tenantId);

        if (tenant is null)
        {
            throw new KeyNotFoundException("الشركة غير موجودة.");
        }

        if (requireActive && !tenant.IsActive)
        {
            throw new InvalidOperationException(
                "لا يمكن تغيير مستودعات شركة غير مفعّلة.");
        }
    }

    private static string ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("اسم المستودع مطلوب.");
        }

        var trimmedName = name.Trim();

        if (trimmedName.Length < 2 || trimmedName.Length > 100)
        {
            throw new ArgumentException(
                "اسم المستودع يجب أن يكون بين حرفين و100 حرف.");
        }

        return trimmedName;
    }

    private static WarehouseDto ToDto(Warehouse warehouse)
    {
        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            TenantId = warehouse.TenantId
        };
    }
}
