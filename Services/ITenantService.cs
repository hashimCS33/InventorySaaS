using System;
using InventorySaaS.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace InventorySaaS.Services;

public interface ITenantService
{
     
    Task<IEnumerable<TenantDto>> GetAllTenantsAsync();

    Task<TenantDto?> GetTenantByIdAsync(int id);

    Task<TenantDto> CreateTenantAsync(CreateTenantDto createTenantDto);

    Task<bool> UpdateTenantAsync(int id, UpdateTenantDto updateTenantDto);
    
    Task<bool> DeleteTenantAsync(int id, string name);
}
