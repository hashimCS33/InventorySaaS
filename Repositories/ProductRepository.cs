using System;
using Microsoft.EntityFrameworkCore;
using InventorySaaS.Models;
using InventorySaaS.Data;
namespace InventorySaaS.Repositories;


public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Product>> GetAllProductsAsync(int tenantId)
    {
        return await _context.
        Set<Product>().AsNoTracking().Where
        (p => p.TenantId == tenantId).ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int tenantId,int id)
    {
        return
         await _context.Set<Product>().AsNoTracking()
         .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == id);
    }

    public async Task<Product?> GetProductBySkuAsync(int tenantId, string sku)
    {
        return await 
        _context.Set<Product>()
        .AsNoTracking().
        FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Sku == sku);
    }

    public async Task AddProductAsync(Product product)
    {
        _context.Set<Product>().Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateProductAsync(int tenantId, Product product)
    {
        var existingProduct = await _context.Set<Product>()
        .FirstOrDefaultAsync
        ( item => item.TenantId == tenantId
          && 
          item.Id == product.Id);



        if (existingProduct == null) return false;

        existingProduct.Name = product.Name;
        existingProduct.Sku = product.Sku;
        existingProduct.Price = product.Price;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteProductAsync(int tenantId,int id)
    {
        var product = await _context.Set<Product>().FirstOrDefaultAsync(item =>
            item.TenantId == tenantId 
            && item.Id == id
            );
        if (product == null) return false;

        _context.Set<Product>().Remove(product);

        await _context.SaveChangesAsync();

        
        return true;
    }
}
