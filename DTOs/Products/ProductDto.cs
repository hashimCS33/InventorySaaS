using System;

namespace InventorySaaS.DTOs.Products;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Sku { get; set; } = null!;
    public decimal? Price { get; set; }
    
    public int TenantId { get; set; }

}
