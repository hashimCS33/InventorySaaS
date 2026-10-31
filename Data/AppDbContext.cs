using System;
using Microsoft.EntityFrameworkCore;
using InventorySaaS.Models;
namespace InventorySaaS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Tenant> Tenants { get; set; } = null!;

    public DbSet<Warehouse> Warehouses { get; set; } = null!;

    public DbSet<StockBalance> StockBalances { get; set; } = null!;


    public DbSet<StockMovement> StockMovements { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
           .Property(product => product.Price)
           .HasPrecision(18, 2);

        modelBuilder.Entity<Product>()
            .HasIndex(product => new { product.TenantId, product.Sku })
            .IsUnique();

        // نستخدم رقم الشركة مع رقم المنتج للربط.
        modelBuilder.Entity<Product>()
            .HasAlternateKey(product => new
            {
                product.TenantId,
                product.Id
            });

        // نستخدم رقم الشركة مع رقم المستودع للربط.
        modelBuilder.Entity<Warehouse>()
            .HasAlternateKey(warehouse => new
            {
                warehouse.TenantId,
                warehouse.Id
            });

        // علاقة الرصيد بالشركة.
        modelBuilder.Entity<StockBalance>()
            .HasOne(balance => balance.Tenant)
            .WithMany()
            .HasForeignKey(balance => balance.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // علاقة الرصيد بالمنتج ضمن نفس الشركة.
        modelBuilder.Entity<StockBalance>()
            .HasOne(balance => balance.Product)
            .WithMany()
            .HasForeignKey(balance => new
            {
                balance.TenantId,
                balance.ProductId
            })
            .HasPrincipalKey(product => new
            {
                product.TenantId,
                product.Id
            })
            .OnDelete(DeleteBehavior.Restrict);

        // علاقة الرصيد بالمستودع ضمن نفس الشركة.
        modelBuilder.Entity<StockBalance>()
            .HasOne(balance => balance.Warehouse)
            .WithMany()
            .HasForeignKey(balance => new
            {
                balance.TenantId,
                balance.WarehouseId
            })
            .HasPrincipalKey(warehouse => new
            {
                warehouse.TenantId,
                warehouse.Id
            })
            .OnDelete(DeleteBehavior.Restrict);

        // سجل رصيد واحد لكل منتج في كل مستودع.
        modelBuilder.Entity<StockBalance>()
            .HasIndex(balance => new
            {
                balance.TenantId,
                balance.ProductId,
                balance.WarehouseId
            })
            .IsUnique();

        // الكمية تدعم ثلاث منازل عشرية.
        modelBuilder.Entity<StockBalance>()
            .Property(balance => balance.Quantity)
            .HasPrecision(18, 3);

        // منع تخزين كمية سالبة.
        modelBuilder.Entity<StockBalance>()
            .ToTable("StockBalances", table =>
                table.HasCheckConstraint(
                    "CK_StockBalances_Quantity_NonNegative",
                    "[Quantity] >= 0"));

        // الحركة تابعة لشركة واحدة.
        modelBuilder.Entity<StockMovement>()
            .HasOne(movement => movement.Tenant)
            .WithMany()
            .HasForeignKey(movement => movement.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // المنتج تابع لنفس شركة الحركة.
        modelBuilder.Entity<StockMovement>()
            .HasOne(movement => movement.Product)
            .WithMany()
            .HasForeignKey(movement => new
            {
                movement.TenantId,
                movement.ProductId
            })
            .HasPrincipalKey(product => new
            {
                product.TenantId,
                product.Id
            })
            .OnDelete(DeleteBehavior.Restrict);

        // المستودع تابع لنفس شركة الحركة.
        modelBuilder.Entity<StockMovement>()
            .HasOne(movement => movement.Warehouse)
            .WithMany()
            .HasForeignKey(movement => new
            {
                movement.TenantId,
                movement.WarehouseId
            })
            .HasPrincipalKey(warehouse => new
            {
                warehouse.TenantId,
                warehouse.Id
            })
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockMovement>()
         .Property(movement => movement.Quantity)
         .HasPrecision(18, 3);

        modelBuilder.Entity<StockMovement>()
            .ToTable("StockMovements", table =>
            {
                table.HasCheckConstraint(
                    "CK_StockMovements_Quantity_Positive",
                    "[Quantity] > 0");

                table.HasCheckConstraint(
                    "CK_StockMovements_Type_Valid",
                    "[Type] IN (1, 2)");
            });

    }
}
