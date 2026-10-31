using System;
using InventorySaaS.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventorySaaS.Repositories;
using InventorySaaS.Models;
using Microsoft.AspNetCore.Http.HttpResults;
namespace InventorySaaS.Services;

public class TenantService : ITenantService
{
     private readonly ITenantRepository _Repository;

     public TenantService(ITenantRepository tenantRepository)
    {
        _Repository = tenantRepository;
    }
   
    public async Task<TenantDto> CreateTenantAsync(CreateTenantDto createTenantDto)
    {
       var name = ValidateName(createTenantDto.Name);
       var tenant = new Tenant
       {
           Name = name,
           IsActive = true
       };
       await _Repository.AddTenantAsync(tenant);
       return ToDto(tenant);
    }

    public async Task<bool> DeleteTenantAsync(int id, string name)
    {
         var tenant = await _Repository.GetTenantByIdAsync(id);

         if (tenant == null) return false;

         await _Repository.DeleteTenantAsync(id, name);
         return true;
    }

    public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync()
    {  
         var tenants = await _Repository.GetAllTenantsAsync();
         return tenants.Select(tenant => new TenantDto
         {
             id = tenant.Id,
             Name = tenant.Name,
             isActive = tenant.IsActive
         }).ToList();
    }

    public  async Task<TenantDto?> GetTenantByIdAsync(int id)
    {
        var tenant = await _Repository.GetTenantByIdAsync(id);
        if (tenant == null) return null;
        return ToDto(tenant);
    }

    public async Task<bool> UpdateTenantAsync(int id, UpdateTenantDto updateTenantDto)
    {
        var tenant = await _Repository.GetTenantByIdAsync(id);

        if (tenant == null) return false;

        var name = ValidateName(updateTenantDto.Name);

        if (updateTenantDto.isActive == null)
        {
            throw new ArgumentException("IsActive must be specified.");
        }

        tenant.Name = name;
        tenant.IsActive = updateTenantDto.isActive.Value;

        await _Repository.UpdateTenantAsync(tenant);
        return true;
    }



    private TenantDto ToDto(Tenant tenant)
    {
        return new TenantDto
        {
            id  = tenant.Id,
            Name = tenant.Name,
            isActive = tenant.IsActive
            
        };
    }

     private static string ValidateName(string? name)   
     {
         if (string.IsNullOrWhiteSpace(name))
         {
             throw new ArgumentException("required name company");
         }
         var trimmedName = name.Trim();

         if(trimmedName.Length > 20)
         {
             throw new ArgumentException("Name must be at most 20 characters long.");
         }


         return trimmedName;
     }

   
}

