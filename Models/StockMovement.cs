using System;
using InventorySaaS.Models.Enums;

namespace InventorySaaS.Models;

public class StockMovement
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int WarehouseId { get; set; }

    public Warehouse Warehouse { get; set; } = null!;

    public decimal Quantity { get; set; }

    public StockMovementType Type { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public Guid? TransferId { get; set; }
}
