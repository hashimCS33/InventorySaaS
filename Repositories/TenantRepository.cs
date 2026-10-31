using System;
using InventorySaaS.Models;
using InventorySaaS.Data;
using Microsoft.EntityFrameworkCore;

namespace InventorySaaS.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly AppDbContext _context;

    public TenantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddTenantAsync(Tenant tenant)
    {
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTenantAsync(int id, string name)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null)
        {
           throw new InvalidOperationException($"Tenant with ID {id} not found.");
        }
        if (tenant.Name != name)
        {
           throw new InvalidOperationException($"Tenant name does not match.");
        }
        var hasproduct = await _context.Products.AnyAsync(p => p.TenantId == id);
        if (hasproduct)
        {
            throw new InvalidOperationException($"Tenant with ID {id} has associated products and cannot be deleted.");
        }
        _context.Tenants.Remove(tenant);
        await _context.SaveChangesAsync();
    }
    

    public async Task<IEnumerable<Tenant>> GetAllTenantsAsync()
    {
        return await _context.Tenants
        .AsNoTracking().ToListAsync();
    }

    public async Task<Tenant?> GetTenantByIdAsync(int id)
    {
        return await _context.Tenants
        .AsNoTracking().
        FirstOrDefaultAsync(tenant => tenant.Id == id);
    }

    public async Task<Tenant?> GetTenantByNameAsync(string name)
    {
        return await _context.Tenants
        .AsNoTracking().
        FirstOrDefaultAsync(tenant => tenant.Name == name);
    }

    public Task UpdateTenantAsync(Tenant tenant)
    {
        _context.Tenants.Update(tenant);
        return _context.SaveChangesAsync();
    }
}
