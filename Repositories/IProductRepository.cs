using System;
using InventorySaaS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace InventorySaaS.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProductsAsync(int tenantId);

    Task<Product?> GetProductByIdAsync(int tenantId,int id);

    Task<Product?> GetProductBySkuAsync(int tenantId, string sku);

    Task AddProductAsync(Product product);

    Task<bool> UpdateProductAsync(int tenantId, Product product);

    Task<bool> DeleteProductAsync(int tenantId,int id);

}
