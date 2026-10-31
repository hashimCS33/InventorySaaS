using System;

namespace InventorySaaS.Models;

public class Tenant
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;



}
