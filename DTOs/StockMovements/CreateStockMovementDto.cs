using System;
using System.ComponentModel.DataAnnotations;
using InventorySaaS.Models.Enums;

namespace InventorySaaS.DTOs.StockMovements;

public class CreateStockMovementDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int WarehouseId { get; set; }

    [Required]
    [EnumDataType(typeof(StockMovementType))]
    public StockMovementType? Type { get; set; }

    [Required]
    [Range(typeof(decimal), "0.001", "999999999999999.999")]
    public decimal? Quantity { get; set; }
}
