using System;
using InventorySaaS.Repositories;
using InventorySaaS.DTOs.Products;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventorySaaS.Models;
namespace InventorySaaS.Services;

public class ProductService: IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ITenantRepository _tenantRepository;

    public ProductService(IProductRepository productRepository, ITenantRepository tenantRepository)
    {
        _productRepository = productRepository;
        _tenantRepository = tenantRepository;
    }

    // Implement the IProductService methods here
     
   public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(int tenantId)
   {
       await ValidateTenantAsync(tenantId);
       
       var products = await _productRepository.GetAllProductsAsync(tenantId);
       
       return products.Select(ToDto).ToList();
   } 


   public async Task<ProductDto> GetProductByIdAsync(int tenantId, int id)
   {
    await ValidateTenantAsync(tenantId);
    var product = await _productRepository.GetProductByIdAsync(tenantId, id);

     return product is null ? null : ToDto(product);
   }

    public async Task<ProductDto> CreateProductAsync(int tenantId, CreateProductDto dto)
    {
        await ValidateTenantAsync(tenantId , requireActive: true);

        var values = ValidateProduct(dto.Name, dto.Sku, dto.Price);

        var existingProduct = await _productRepository.GetProductBySkuAsync(tenantId, values.sku); 
        if (existingProduct != null)
            throw new ArgumentException("Product with the same SKU already exists.");

        var product = new Product
        {
            Name = values.Name,
            Sku = values.sku,
            Price = values.Price,
            TenantId = tenantId
        };

        await _productRepository.AddProductAsync(product);
        return ToDto(product);

    }

    public async Task<bool> UpdateProductAsync(int tenantId, int id, UpdateProductDto dto)
  {
      await ValidateTenantAsync(tenantId, requireActive: true);

      var  product = await _productRepository.GetProductByIdAsync(tenantId, id);

      if (product == null)
          return false;

      var values = ValidateProduct(
        dto.Name, 
        dto.Sku, 
        dto.Price);

      var existingProductWithSku = await _productRepository.GetProductBySkuAsync(tenantId, values.sku);
      
      
      if (existingProductWithSku != null && existingProductWithSku.Id != product.Id)
         
          throw new ArgumentException("Product with the same SKU already exists.");

      product.Name = values.Name;
      product.Sku = values.sku;
      product.Price = values.Price;
    
    

    return await _productRepository.UpdateProductAsync(tenantId, product);
  }

  public async Task<bool> DeleteProductAsync(int tenantId, int id)
  {
      await ValidateTenantAsync(tenantId, requireActive: true);

      return await _productRepository.DeleteProductAsync(tenantId, id);
  }


private async Task ValidateTenantAsync(int tenantId, bool requireActive = false)
{
    var tenant = await _tenantRepository.GetTenantByIdAsync(tenantId);
    if (tenant == null)
        throw new ArgumentException("not found tenant.");

    if (requireActive && !tenant.IsActive)
        throw new ArgumentException("Tenant is not active.");
}


    private (string Name,string sku,decimal Price) ValidateProduct(string? name, string? sku, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name most be provided.");
        
        
        if (string.IsNullOrWhiteSpace(sku))
           
            throw new ArgumentException("Product SKU cannot most be provided.");
       
        var trimmedName = name.Trim();
        var trimmedSku = sku.Trim();

        if (trimmedName.Length > 200)
            throw new ArgumentException("Product name cannot exceed 200 characters.");

        if (trimmedSku.Length > 50)
            throw new ArgumentException("Product SKU cannot exceed 50 characters.");

        if (price  <= 0 || price > 1000000)
            throw new ArgumentException("sale price is excessively high.");

        if (decimal.Round(price, 2) != price)
            throw new ArgumentException("Product price cannot have more than two decimal places.");

        return (trimmedName, trimmedSku, price);
    }


   
   private static ProductDto ToDto(Product product)
   {
       return new ProductDto
       {
           Id = product.Id,
           Name = product.Name,
           Sku = product.Sku,
           Price = product.Price,
           TenantId = product.TenantId
       };
   }

   
}