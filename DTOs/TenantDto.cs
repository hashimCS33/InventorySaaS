using System;

namespace InventorySaaS.DTOs;

public class TenantDto
{
    public int id {get; set;}


    public String Name { get; set; } = string.Empty;

    public bool? isActive { get; set; }

}
