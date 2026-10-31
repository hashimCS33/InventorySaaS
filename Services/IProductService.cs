using System;
using InventorySaaS.DTOs.Products;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace InventorySaaS.Services;

public interface IProductService
{
  Task<IEnumerable<ProductDto>> GetAllProductsAsync(int tenantId);

  Task<ProductDto> GetProductByIdAsync(int tenantId, int id);

  Task<ProductDto> CreateProductAsync(int tenantId, CreateProductDto dto);

  Task<bool> UpdateProductAsync(int tenantId, int id, UpdateProductDto dto);

  Task<bool> DeleteProductAsync(int tenantId, int Id);
}
