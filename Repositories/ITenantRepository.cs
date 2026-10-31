using System;
using InventorySaaS.Models;

namespace InventorySaaS.Repositories;

public interface ITenantRepository
{
    Task<IEnumerable<Tenant>> GetAllTenantsAsync();

    Task<Tenant?> GetTenantByIdAsync(int id);
    
    Task<Tenant?> GetTenantByNameAsync(string name);

    Task AddTenantAsync(Tenant tenant);

    Task UpdateTenantAsync(Tenant tenant);
    
    Task DeleteTenantAsync(int id, string name);

}
