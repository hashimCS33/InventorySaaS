using System;
using System.ComponentModel.DataAnnotations;

namespace InventorySaaS.DTOs.Products;

public class UpdateProductDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Sku { get; set; } = null!;

    [Required]
    [Range(typeof(decimal),
     "0", "9999999999999999.99",
    ErrorMessage = "Price must 0 or greater")]
    public decimal Price { get; set; }

}
