using System;
using System.ComponentModel.DataAnnotations;

namespace InventorySaaS.DTOs.Warehouses;

public class UpdateWarehouseDto
{
   
   [Required]
   [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;
}
