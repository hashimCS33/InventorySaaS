using System;
using System.ComponentModel.DataAnnotations;

namespace InventorySaaS.DTOs.StockMovements;

public class TransferStockDto
{
   [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0.")]
   public int productId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Source Warehouse ID must be greater than 0.")]
    public int sourceWarehouseId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Destination Warehouse ID must be greater than 0.")]
    public int destinationWarehouseId { get; set; }
    [Required(ErrorMessage = "Transfer date is required.")]
    [Range(typeof(decimal), "0.001", "999999999999999.999", ErrorMessage = "Quantity must be greater than 0.")]
    public decimal? quantity { get; set; }
}
