using System;

namespace InventorySaaS.Models;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public String Sku { get; set; } = string.Empty;

    public int TenantId { get; set; }


    public Tenant Tenant { get; set; } = null!;

    public decimal Price { get; set; }
}
