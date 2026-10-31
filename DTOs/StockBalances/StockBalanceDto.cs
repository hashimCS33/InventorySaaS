using System;

namespace InventorySaaS.DTOs.StockBalances;

public class StockBalanceDto
{
     public int Id { get; set; }

    public int TenantId { get; set; }

    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public decimal Quantity { get; set; }
}
