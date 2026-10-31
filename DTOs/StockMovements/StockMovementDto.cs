using System;
using InventorySaaS.Models.Enums;

namespace InventorySaaS.DTOs.StockMovements;

public class StockMovementDto
{ 

    public int Id { get; set; }

    public int TenantId { get; set; }

    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public StockMovementType Type { get; set; }

    public decimal Quantity { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public Guid? TransferId { get; set; }

}
