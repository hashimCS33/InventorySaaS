using System;
using System.ComponentModel.DataAnnotations;
namespace InventorySaaS.DTOs;

public class UpdateTenantDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

   [Required]
    public bool? isActive { get; set; }

}
