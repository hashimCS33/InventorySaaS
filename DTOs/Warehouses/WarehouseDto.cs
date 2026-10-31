using System;

namespace InventorySaaS.DTOs.Warehouses;

public class WarehouseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public int TenantId { get; set; }
}
